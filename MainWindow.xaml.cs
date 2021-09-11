using System;
using System.Windows;
using System.Linq;
using MahApps.Metro.Controls;
using System.Drawing;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using StickyWindows;
using StickyWindows.WPF;
using ControlzEx;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;


namespace QuickLauncher
{
    // The extra libraries needed to run this app are included as an embedded resource so the app can run as a single .exe file in a folder without dll's.
    // These are loaded in the App.xaml.cs > Main() with AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>

    // This project also uses the 2 following library (https://www.nuget.org/packages/StickyWindows.WPF/0.3.0-unstable0009 and https://www.nuget.org/packages/PixiEditor.ColorPicker) but is not installed with the package manager.
    // To include libraries as resource they need to be strongly named but StickyWindows was not when installing from nuget. And PixiEditor did not work from nuget but did when compiling it myself.


    public partial class MainWindow : MetroWindow
    {
        private StickyWindow StickyWindow;
        private TaskbarIcon TaskbarIcon;
        public static Classes.Setting.Settings Settings;
        public bool IsFirstLoad;
        public int NextStartRow;
        public bool SaveSettingsOnExit;
        public bool IsEditMode;

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                //make windows sticky
                this.Loaded += MakeStickyWindow;

                //initen global variables
                Classes.Globals.InitGlobals();

                //styles and texts
                SetStylesAndTexts();

                //init app specific code
                InitAppStuff(false);

                //create the taskbar icon
                CreateTaskBarIcon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Localizer.GetLocalized("mainwindow-error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #region init


        /// <summary>
        /// Sets the window styles and localized texts
        /// </summary>
        private void SetStylesAndTexts()
        {
            //title
            this.Title = $"{Classes.Globals.AppDeveloper} {Classes.Globals.AppName}";

            //tooltip text in title bar
            WindowButtons.Minimize = Localizer.GetLocalized("mainwindow-minimize");
            WindowButtons.Maximize = Localizer.GetLocalized("mainwindow-maximize");
            WindowButtons.Restore = Localizer.GetLocalized("mainwindow-restore");
            WindowButtons.Close = Localizer.GetLocalized("mainwindow-close");

            //header buttons
            button_about.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.About, Classes.ResourceController.BrushWhite, null);
            button_pinned.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Pin, Classes.ResourceController.BrushWhite, null);
            button_unpinned.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Unpin, Classes.ResourceController.BrushWhite, null);

            //header button tooltips
            tt_overAppHeader.Content = Localizer.GetLocalized("mainwindow-about");
            tt_Pinned.Content = Localizer.GetLocalized("mainwindow-ontop");
            tt_UnPinned.Content = Localizer.GetLocalized("mainwindow-ontop");
            tt_Button_settings.Content = Localizer.GetLocalized("settings-tooltip-settings");
            tt_Button_add.Content = Localizer.GetLocalized("settings-tooltip-add");
            tt_Button_edit.Content = Localizer.GetLocalized("settings-tooltip-edit");

            //bottom settings buttons
            button_settings.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Settings, Classes.ResourceController.BrushGray, null, 0.5d);
            button_add.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Add, Classes.ResourceController.BrushGray, null, 0.5d);
            button_edit.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Edit, Classes.ResourceController.BrushGray, null, 0.5d);

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;
            }
        }


        /// <summary>
        /// Creates the taskbar icon with a context menu
        /// </summary>
        public void CreateTaskBarIcon()
        {
            //cerate the taskbar icon
            TaskbarIcon = new TaskbarIcon()
            {
                ToolTipText = this.Title,
                Icon = new Icon(Classes.ResourceController.GetStreamFromResource("favicon_taskbar.ico")),
                ContextMenu = new ContextMenu()
                {
                    HasDropShadow = false
                }
            };

            //add items to the menu
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.Maximize, Localizer.GetLocalized("mainwindow-maximize"), Contextmenu_maximize_Click));
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.About, Localizer.GetLocalized("mainwindow-about"), Button_about_Click));
            TaskbarIcon.ContextMenu.Items.Add(new Separator());
            TaskbarIcon.ContextMenu.Items.Add(Classes.ResourceController.CreateContextMenuItem(Classes.Enums.Icon.Close, Localizer.GetLocalized("mainwindow-close"), Contextmenu_close_Click));

            //insert it at the top of the dockpanel
            dockpanel_main.Children.Insert(0, TaskbarIcon);
        }


        /// <summary>
        /// Start some app specific stuff
        /// </summary>
        public void InitAppStuff(bool FromSettingsSave)
        {
            IsFirstLoad = true;

            //load settings
            Settings = Classes.Setting.Load();

            //declare some variables
            int GridMargin = 10;
            int Dimension = Settings.button_size;
            int Separator = 10;
            int RowHeightGroupSeparator = Settings.group_spacing;
            int HeaderHeight = 32;
            int SystemButtonRowHeight = 35;
            int MinHeight = (Dimension * 2) + RowHeightGroupSeparator + (GridMargin * 2) + HeaderHeight;

            //clear the grid if the settings were saved
            if (FromSettingsSave)
            {
                grid_main.Children.Clear();

                //reset the rows and columns
                if (grid_main.ColumnDefinitions.Count > 0)
                {
                    grid_main.ColumnDefinitions.RemoveRange(0, grid_main.ColumnDefinitions.Count);
                }
                if (grid_main.RowDefinitions.Count > 0)
                {
                    grid_main.RowDefinitions.RemoveRange(0, grid_main.RowDefinitions.Count);
                }

                NextStartRow = 0;
            }
            else if (Settings.always_on_top)
            {
                //pin the app if stored in settings
                Button_pin_Click(null, null);
            }

            //create the grid columns
            for (int i = 0; i < Settings.columns; i++)
            {
                if (i > 0)
                {
                    grid_main.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(Separator)
                    });
                }

                grid_main.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(Dimension)
                });
            }

            //calculate the total number of rows
            int rows = 0;
            int height = 0;

            //create the grid rows
            for (int i = 0; i < Settings.groups; i++)
            {
                //shortcuts per group
                var shortcuts = Settings.shortcuts.Where(x => x.group_int == i + 1).ToList();

                //no items in group then skip
                if (shortcuts.Count == 0)
                {
                    continue;
                }

                //how many rows are needed for this group
                var rows_per_group = Math.Ceiling((decimal)shortcuts.Count() / Settings.columns);

                for (int j = 0; j < rows_per_group; j++)
                {
                    //row separator
                    if (j > 0)
                    {
                        grid_main.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(Separator)
                        });

                        height += Separator;
                    }

                    //normal row
                    grid_main.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(Dimension)
                    });

                    rows++;
                    height += Dimension;
                }

                //group separator row
                if (Settings.groups > 1 && i + 1 < Settings.groups)
                {
                    grid_main.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(RowHeightGroupSeparator)
                    });

                    height += RowHeightGroupSeparator;
                }

                //create the shortcuts
                CreateShortCuts((int)rows_per_group, shortcuts);
            }

            //width of the app. The 2 px extra are needed because of the outer border
            this.Width = (Settings.columns * (Dimension + Separator) - Separator) + (GridMargin * 2) + 2;

            //height of the app
            height = height + (GridMargin * 2) + HeaderHeight + SystemButtonRowHeight;
            if (height < MinHeight)
                height = MinHeight;

            this.MinHeight = MinHeight;
            this.Height = height;

            //hide edit button if there are no shortcuts
            if (Settings.shortcuts.Count == 0)
            {
                button_edit.Visibility = Visibility.Hidden;
            }
            else
            {
                button_edit.Visibility = Visibility.Visible;
            }

            IsFirstLoad = false;
        }


        /// <summary>
        /// Creates all the shortcut buttons in the grid
        /// </summary>
        /// <param name="rows_per_group">The total number of rows needed for the group</param>
        /// <param name="shortcuts">List of shortcuts of a group</param>
        public void CreateShortCuts(int rows_per_group, List<Classes.Setting.ShortCut> shortcuts)
        {
            //variables
            int row = NextStartRow;
            int col = 0;
            var iconHeight = Math.Ceiling(Settings.button_size * 0.666);
            var questionIcon = Geometry.Parse(Classes.IconController.IconList().Where(x => x.icon == Classes.Enums.Icon.Questionmark).FirstOrDefault().path);
            var shortcutIcon = questionIcon;
            System.Windows.Media.Brush IconColor = Classes.ResourceController.BrushBlack;
            System.Windows.Media.SolidColorBrush ButtonColor = Classes.ResourceController.BrushDefaultButton;

            //some styles
            var StyleTooltip = this.FindResource("ToolTip_ShortCut") as Style;
            var StyleButton = this.FindResource("Button_ShortCut") as Style;

            //the next group row
            NextStartRow += rows_per_group + 1;

            for (int i = 0; i < shortcuts.Count; i++)
            {
                var item = shortcuts[i];

                //try to get the hex background color for the button
                if (!string.IsNullOrEmpty(item.color_button))
                {
                    try
                    {
                        ButtonColor = (SolidColorBrush)new BrushConverter().ConvertFromString(item.color_button);
                    }
                    catch
                    {
                        //make button red if incorrect color
                        ButtonColor = Classes.ResourceController.BrushRed;
                    }
                }

                //try to get the hex background color for the button
                if (!string.IsNullOrEmpty(item.color_icon))
                {
                    try
                    {
                        IconColor = (SolidColorBrush)new BrushConverter().ConvertFromString(item.color_icon);
                    }
                    catch
                    {
                    }
                }

                //try to get the icon path
                if (!string.IsNullOrEmpty(item.icon))
                {
                    try
                    {
                        shortcutIcon = Geometry.Parse(item.icon);
                    }
                    catch
                    {
                        shortcutIcon = questionIcon;
                    }
                }

                //go to the next row if the max per row is reached
                if (col > Settings.columns - 1)
                {
                    row = row + 2;
                    col = 0;

                    //if the group needs more rows then increment
                    NextStartRow++;
                }

                //create the tooltip
                var tooltip = new ToolTip()
                {
                    Content = item.name,
                    Style = StyleTooltip,
                    Background = ButtonColor,
                    BorderBrush = Classes.ResourceController.BrushBlack,
                    Foreground = IconColor
                };

                ToolTipAssist.SetAutoMove(tooltip, true);

                //create the icon
                var path = new System.Windows.Shapes.Path()
                {
                    Fill = IconColor,
                    Data = shortcutIcon
                };

                var viewBox = new Viewbox()
                {
                    Height = iconHeight,
                    Width = iconHeight
                };

                var canvas = new Canvas()
                {
                    Width = 24,
                    Height = 24
                };

                canvas.Children.Add(path);
                viewBox.Child = canvas;

                ////create a linear gradient (not implemented). To use set "Background = linear" in the button shorthand,
                //var GradientColor1 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(item.color_button);
                //var GradientColor2 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(item.color_icon);

                //var linear = new LinearGradientBrush()
                //{
                //    StartPoint = new System.Windows.Point(0.5, 0),
                //    EndPoint = new System.Windows.Point(0.5, 1.5)
                //};
                //linear.GradientStops.Add(new GradientStop()
                //{
                //    Color = GradientColor1,
                //    Offset = 0
                //});
                //linear.GradientStops.Add(new GradientStop()
                //{
                //    Color = GradientColor2,
                //    Offset = 1
                //});

                //create the button
                var button = new Button()
                {
                    Focusable = false,
                    ToolTip = tooltip,
                    Background = ButtonColor,
                    Name = "button_" + item.id, //a name cannot be just a number, so a prefix is needed
                    Style = StyleButton
                };

                button.Content = viewBox;

                //add the click handler
                button.Click += Button_grid_Click;

                //add the button to the grid
                Grid.SetRow(button, row);
                Grid.SetColumn(button, col * 2);
                grid_main.Children.Add(button);

                col++;
            }
        }


        #endregion


        #region various


        /// <summary>
        /// makes the window snap to the edges of the screen
        /// </summary>
        void MakeStickyWindow(object sender, RoutedEventArgs e)
        {
            StickyWindow = this.CreateStickyWindow();
        }


        /// <summary>
        /// Mazimizes the window
        /// </summary>
        private void NormalWindow()
        {
            this.Show();
            this.Activate();
            this.WindowState = WindowState.Normal;
        }


        /// <summary>
        /// Closes the app
        /// </summary>
        public void CloseApp()
        {
            //save the settings before close to store the clicks
            if (SaveSettingsOnExit)
            {
                Settings.Save();
            }

            if (TaskbarIcon != null)
            {
                TaskbarIcon.Dispose();
            }

            Application.Current.Shutdown();
            Environment.Exit(0);
        }


        /// <summary>
        /// State changed handler
        /// </summary>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }


        /// <summary>
        /// When the main window closes make sure to execute closeapp to save the settings
        /// </summary>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseApp();
        }


        #endregion


        #region menus


        /// <summary>
        /// Mazimizes the window from the context menu
        /// </summary>
        private void Contextmenu_maximize_Click(object Sender, EventArgs e)
        {
            NormalWindow();
        }


        /// <summary>
        /// Closes the app from the context menu
        /// </summary>
        private void Contextmenu_close_Click(object Sender, EventArgs e)
        {
            CloseApp();
        }


        /// <summary>
        /// Opens the about windows from the context menu
        /// </summary>
        private void Button_about_Click(object Sender, EventArgs e)
        {
            NormalWindow();

            var about = new OverApp()
            {
                Owner = Application.Current.MainWindow
            };

            about.ShowDialog();
        }


        /// <summary>
        /// Sets the window to the top or not
        /// </summary>
        private void Button_pin_Click(object Sender, EventArgs e)
        {
            if (this.Topmost)
            {
                this.Topmost = false;
                button_pinned.Visibility = Visibility.Collapsed;
                button_unpinned.Visibility = Visibility.Visible;

                Settings.always_on_top = false;
            }
            else
            {
                this.Topmost = true;
                button_pinned.Visibility = Visibility.Visible;
                button_unpinned.Visibility = Visibility.Collapsed;

                Settings.always_on_top = true;
            }

            //do not save the changes if triggered from MainWindow()
            if (IsFirstLoad)
                return;

            Settings.Save();
        }


        #endregion


        #region buttons


        /// <summary>
        /// Opens the app settings window
        /// </summary>
        private void Button_settings_Click(object sender, EventArgs e)
        {
            var settings = new Settings()
            {
                Owner = Application.Current.MainWindow
            };

            settings.ShowDialog();
        }


        /// <summary>
        /// opens the add new shortcut window
        /// </summary>
        private void button_add_Click(object sender, RoutedEventArgs e)
        {
            var editwindow = new EditItem(null)
            {
                Owner = Application.Current.MainWindow
            };

            editwindow.ShowDialog();
        }


        /// <summary>
        /// Sets the window in edit mode so when a shortcut is clicked the edit window pops up
        /// </summary>
        private void button_edit_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                button_edit.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Edit, Classes.ResourceController.BrushGray, null, 0.5d);
            }
            else
            {
                //change the color of the edit button to indicate edit mode
                var brush = Classes.ResourceController.BrushBlack;
                if (Classes.Globals.IsDarkMode())
                {
                    brush = Classes.ResourceController.BrushWhite;
                }

                button_edit.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Edit, Classes.ResourceController.BrushRed, null);
            }

            IsEditMode = !IsEditMode;
        }


        /// <summary>
        /// Handles the button clicks from the shortcuts in the grid
        /// </summary>
        private void Button_grid_Click(object sender, EventArgs e)
        {
            var button = sender as Button;

            var shortcut = Settings.shortcuts.Where(x => x.id == Convert.ToInt32(button.Name.Replace("button_", ""))).FirstOrDefault();

            //is edit mode
            if (IsEditMode)
            {
                button_edit_Click(null, null);

                //open the edit window
                var editwindow = new EditItem(shortcut)
                {
                    Owner = Application.Current.MainWindow
                };

                editwindow.ShowDialog();

                return;
            }

            string path = shortcut.executable_path;

            //check if email
            if (Classes.Setting.IsValidEmail(path))
            {
                path = "mailto:" + path;
            }

            try
            {
                Process.Start(path, string.IsNullOrEmpty(shortcut.executable_arguments) ? null : shortcut.executable_arguments);

                //trigger save settings on exit
                SaveSettingsOnExit = true;

                //update some stats
                shortcut.clicks_int++;
                shortcut.date_last_used = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\nPath: {shortcut.executable_path}", Localizer.GetLocalized("mainwindow-error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        #endregion
    }
}
