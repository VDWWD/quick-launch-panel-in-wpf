﻿using System;
using System.Collections.Generic;
using Microsoft.Shell;
using System.Windows;
using System.Reflection;

namespace QuickLauncher
{
    public partial class App : Application, ISingleInstanceApp
    {
        //to create a single instance app: https://codereview.stackexchange.com/questions/20871/single-instance-wpf-application

        private const string UniqueKey = "76f64c26-d39f-4543-af76-1c2cdbba57fd";

        [STAThread]
        static void Main()
        {
            //if there already is an instance running of this app then quit
            if (!SingleInstance<App>.InitializeAsFirstInstance(UniqueKey))
                return;

            //load the embedded libaries
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                using (var stream = Classes.ResourceController.GetStreamFromResource(new AssemblyName(args.Name).Name + ".dll"))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            var application = new App();
            application.InitializeComponent();
            application.Run();

            SingleInstance<App>.Cleanup();
        }


        //=== toon venster indien nogmaals op exe geklikt ==================================================================================
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                this.MainWindow.Show();
                this.MainWindow.WindowState = WindowState.Normal;
            }

            if (this.MainWindow.Topmost)
            {
                this.MainWindow.Activate();
            }
            else
            {
                this.MainWindow.Topmost = true;
                this.MainWindow.Activate();
                this.MainWindow.Topmost = false;
            }

            return true;
        }
    }
}
