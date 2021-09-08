using System;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace QuickLauncher
{
    public partial class EditItem : MetroWindow
    {
        public Classes.Setting.ShortCut ShortCut { get; set; }
        public bool IsNewIcon { get; set; }

        public EditItem(Classes.Setting.ShortCut shortcut)
        {
            InitializeComponent();
            ShortCut = shortcut;

            //is it an edit or a new shorcut
            if (ShortCut == null)
            {
                IsNewIcon = true;
                ShortCut = new Classes.Setting.ShortCut();

                this.Title = Localizer.GetLocalized("editshortcut-new");

                //if new item hide delete button
                button_delete.Visibility = Visibility.Hidden;
            }
            else
            {
                this.Title = Localizer.GetLocalized("editshortcut-edit") + ShortCut.name;
            }

            //styles and texts
            txt_label1.Text = Localizer.GetLocalized("editshortcut-name");
            txt_label2.Text = Localizer.GetLocalized("editshortcut-exec");
            txt_label3.Text = Localizer.GetLocalized("editshortcut-args");
            txt_label4.Text = Localizer.GetLocalized("editshortcut-icon");
            txt_label5.Text = Localizer.GetLocalized("editshortcut-colorbutton");
            txt_label6.Text = Localizer.GetLocalized("editshortcut-coloricon");
            txt_label7.Text = Localizer.GetLocalized("editshortcut-index");
            txt_label8.Text = Localizer.GetLocalized("editshortcut-group");

            //slider
            slider_groups.Minimum = MainWindow.Settings.MinGroups;
            slider_groups.Maximum = MainWindow.Settings.groups;

            //tooltips
            tt_Button_browse.Content = Localizer.GetLocalized("editshortcut-tooltip-exec");
            tt_Button_icon.Content = Localizer.GetLocalized("editshortcut-tooltip-icon");
            tt_Button_colorpicker1.Content = Localizer.GetLocalized("editshortcut-tooltip-color");
            tt_Button_colorpicker2.Content = Localizer.GetLocalized("editshortcut-tooltip-color");

            //tooltip icon
            button_browse.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Browse, Classes.ResourceController.BrushGray, null, 0.5d);
            button_icon.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.About, Classes.ResourceController.BrushGray, null, 0.5d);
            button_colorpicker1.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Colorpicker, Classes.ResourceController.BrushGray, null, 0.5d);
            button_colorpicker2.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Colorpicker, Classes.ResourceController.BrushGray, null, 0.5d);

            //set the values
            textbox_name.Text = ShortCut.name;
            textbox_exec.Text = ShortCut.executable_path;
            textbox_args.Text = ShortCut.executable_arguments;
            textbox_color_icon.Text = ShortCut.color_icon;
            textbox_index.Text = ShortCut.index;
            slider_groups.Value = ShortCut.group_int;
            if (!IsNewIcon)
            {
                textbox_color_button.Text = ShortCut.color_button;
                textbox_icon.Text = ShortCut.icon;
            }

            //show stats on edit
            if (ShortCut.id > 0)
            {
                txt_dateadded1.Text = Localizer.GetLocalized("editshortcut-date-added");
                txt_dateused1.Text = Localizer.GetLocalized("editshortcut-date-used");
                txt_usages1.Text = Localizer.GetLocalized("editshortcut-usage");

                txt_dateadded2.Text = $"{getDateWithoutWeekday(ShortCut.date_added)}, {ShortCut.date_added.ToShortTimeString()}";
                txt_usages2.Text = ShortCut.clicks;

                //if the login has never been used
                if (ShortCut.date_last_used.Year > 2000)
                {
                    txt_dateused2.Text = $"{getDateWithoutWeekday(ShortCut.date_last_used)}, {ShortCut.date_last_used.ToShortTimeString()}";
                }
            }
            else
            {
                txt_dateadded1.Text = "";
                txt_dateused1.Text = "";
                txt_usages1.Text = "";

                this.Height = this.Height - 85;
            }

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                txt_label1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label3.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label4.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label5.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label6.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label7.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label8.Foreground = Classes.ResourceController.BrushDarkModeText;

                txt_dateadded1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateused1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_usages1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateadded2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_dateused2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_usages2.Foreground = Classes.ResourceController.BrushDarkModeText;

                textbox_name.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_exec.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_args.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_icon.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_color_button.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_color_icon.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                textbox_index.Style = (Style)FindResource("TextBox_Normal_DarkMode");

                button_delete.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Delete, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("editshortcut-delete"));
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("editshortcut-save"));
            }
            else
            {
                button_delete.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Delete, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("editshortcut-delete"));
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("editshortcut-save"));
            }

            //trigger the slider change event for the inital colors
            if (IsNewIcon)
            {
                slider_groups_ValueChanged(null, null);
            }
        }


        /// <summary>
        /// Saves the settings and closes the window
        /// </summary>
        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            bool errors = false;
            string name = textbox_name.Text.Trim();
            string exec = textbox_exec.Text.Trim();
            string icon = textbox_icon.Text.Trim();


            //if textbox is empty or the length is incorrect then make red
            if (string.IsNullOrEmpty(name) || name.Length < 3)
            {
                textbox_name.Style = Classes.ResourceController.StyleTextBoxError;
                errors = true;
            }

            //if textbox is empty or the length is incorrect then make red
            if (string.IsNullOrEmpty(exec) || exec.Length < 5)
            {
                textbox_exec.Style = Classes.ResourceController.StyleTextBoxError;
                errors = true;
            }

            //if textbox is empty or the length is incorrect then make red
            if (string.IsNullOrEmpty(icon) || icon.Length < 5)
            {
                textbox_icon.Style = Classes.ResourceController.StyleTextBoxError;
                errors = true;
            }

            //if there is an error then quit
            if (errors)
            {
                return;
            }

            //set the values
            ShortCut.name = textbox_name.Text.Trim();
            ShortCut.executable_path = textbox_exec.Text.Trim();
            ShortCut.executable_arguments = textbox_args.Text.Trim();
            ShortCut.icon = textbox_icon.Text.Trim();
            ShortCut.color_button = textbox_color_button.Text.Trim();
            ShortCut.color_icon = textbox_color_icon.Text.Trim();
            ShortCut.index = textbox_index.Text.Trim();
            ShortCut.group = Convert.ToInt32(slider_groups.Value).ToString();

            //if new shortcut add it to the list
            if (ShortCut.id == 0)
            {
                ShortCut.date_added = DateTime.Now;

                //check if the list is empty, if not set the id as the current highest + 1
                if (MainWindow.Settings.shortcuts.Count > 0)
                {
                    ShortCut.id = MainWindow.Settings.shortcuts.Max(x => x.id) + 1;
                }
                else
                {
                    ShortCut.id = 1;
                }

                MainWindow.Settings.shortcuts.Add(ShortCut);
                MainWindow.Settings.Sort();
            }

            //save
            MainWindow.Settings.Save();

            //rebuild the shortcuts
            ((MainWindow)Application.Current.MainWindow).InitAppStuff(true);

            this.DialogResult = true;
        }


        /// <summary>
        /// Show login delete confirmation. When yes removes it and saves
        /// </summary>
        private void Button_delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(string.Format(Localizer.GetLocalized("editshortcut-delete-confirm"), ShortCut.name), Localizer.GetLocalized("editshortcut-delete-title"), MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                //remove the shortcut
                MainWindow.Settings.shortcuts.RemoveAll(x => x.id == ShortCut.id);

                //save
                MainWindow.Settings.Save();

                //rebuild the shortcuts
                ((MainWindow)Application.Current.MainWindow).InitAppStuff(true);

                this.DialogResult = true;
            }
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Escape)
            {
                this.DialogResult = true;
            }
        }


        /// <summary>
        /// Changes the textbox style when something is types from red to the normal color it the textbox was empty
        /// </summary>     
        private void textbox_index_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            textbox_index.Text = Regex.Replace(textbox_index.Text, "[^0-9-]", "");
        }


        /// <summary>
        /// Open the openfiledialog to select a file from the disk
        /// </summary>
        private void button_browse_Click(object sender, RoutedEventArgs e)
        {
            //show save file dialog
            var dialog = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = dialog.ShowDialog();

            //dialog ok then save
            if (result == true)
            {
                textbox_exec.Text = dialog.FileName;
            }
        }


        /// <summary>
        /// Open the color picker
        /// </summary>
        private void button_colorpicker_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string hexcolor = textbox_color_icon.Text;
            bool for_button_color = false;
            var color = Colors.Black;

            //is the picker for the button or icon
            if (btn.Name == "button_colorpicker1")
            {
                for_button_color = true;
                hexcolor = textbox_color_button.Text;
            }

            //try to convert the hex to color
            try
            {
                color = (Color)ColorConverter.ConvertFromString(hexcolor);
            }
            catch
            {
            }

            //open the color picker
            var picker = new PickColor(for_button_color, color)
            {
                Owner = this
            };

            picker.ShowDialog();
        }


        /// <summary>
        /// Opens the materialdesign website
        /// </summary>
        private void button_icon_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://materialdesignicons.com");
        }


        /// <summary>
        /// Changes the textbox style when something is types from red to the normal color it the textbox was empty
        /// </summary>     
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;

            if (Classes.Globals.IsDarkMode())
            {
                tb.Style = Classes.ResourceController.StyleTextBoxDarkMode;
            }
            else
            {
                tb.Style = Classes.ResourceController.StyleTextBoxNormal;
            }

            //make sure that if a user pastes an svg image with multiple rows, the svg is made single line. And to prevent just pressing enter in the textbox that must accept multiline
            tb.Text = tb.Text.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
        }


        /// <summary>
        /// Gets the date without weekday in the correct localization: May 27, 2021
        /// </summary>
        /// <param name="value">The DateTime date</param>
        /// <returns>String with the date without weekday</returns>
        public string getDateWithoutWeekday(DateTime date)
        {
            return date.ToLongDateString().Replace(DateTimeFormatInfo.CurrentInfo.GetDayName(Convert.ToDateTime(date).DayOfWeek), "").TrimStart(", ".ToCharArray());
        }


        /// <summary>
        /// On group change find the color of the first icon in that group and use if for the new icon
        /// </summary>
        private void slider_groups_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //do not change the color of existing shortcuts
            if (!IsNewIcon)
            {
                return;
            }

            var old_shortcut = MainWindow.Settings.shortcuts.Where(x => x.group_int == Convert.ToInt32(slider_groups.Value)).FirstOrDefault();

            //if no shortcut is found in the group use the default values, otherwise use the existing colors
            if (old_shortcut == null)
            {
                textbox_color_button.Text = ShortCut.color_button;
                textbox_color_icon.Text = ShortCut.color_icon;
            }
            else
            {
                textbox_color_button.Text = old_shortcut.color_button;
                textbox_color_icon.Text = old_shortcut.color_icon;
            }
        }
    }
}
