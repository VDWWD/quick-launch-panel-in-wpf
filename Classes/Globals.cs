﻿using System;
using Microsoft.Win32;
using System.Globalization;

namespace QuickLauncher.Classes
{
    public class Globals
    {
        public static Enums.Language Language { get; set; }
        public static decimal AppVersion { get; set; }
        public static string AppName { get; set; }
        public static string AppUrl { get; set; }
        public static string AppEmail { get; set; }
        public static string AppCopyright { get; set; }
        public static string AppDeveloper { get; set; }
        public static string AppPath { get; set; }
        public static string AppPathSettingsFile { get; set; }
        public static string AppPathIconPackFile { get; set; }
        private static bool? _IsDarkMode { get; set; }


        /// <summary>
        /// Initialize some globally used variables
        /// </summary>
        public static void InitGlobals()
        {
            //declare some global variables
            AppVersion = 1.5m;
            AppName = "QuickLauncher";
            AppEmail = "erwin@vanderwaal.eu";
            AppUrl = "https://www.vanderwaal.eu";
            AppCopyright = "van der Waal Webdesign";
            AppDeveloper = "VDWWD";
            AppPath = AppDomain.CurrentDomain.BaseDirectory;
            AppPathSettingsFile = string.Format(@"{0}\{1}.settings", AppPath, AppName);
            AppPathIconPackFile = string.Format(@"{0}\{1}.IconPack.json", AppPath, AppName);

            //search the windows language
            var ci = CultureInfo.CurrentUICulture;
            if (ci.Name == "nl-NL" || ci.Name == "nl-BE")
                Language = Enums.Language.NL;
            else if (ci.Name == "de-DE")
                Language = Enums.Language.DE;
            else
                Language = Enums.Language.EN;

            Localizer.CreateLanguageData();
        }


        /// <summary>
        /// Check if the current Windows User is using dark mode
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool IsDarkMode()
        {
            //store the boolean in a variable instead of checking the registery every time. Not sure how fast this is
            if (_IsDarkMode != null)
                return (bool)_IsDarkMode;

            _IsDarkMode = false;

            //get the darkmode settings from the windows registery
            try
            {
                var key = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", "1");
                if (key?.ToString() == "0")
                    _IsDarkMode = true;
            }
            catch
            {
            }

            return (bool)_IsDarkMode;
        }
    }
}
