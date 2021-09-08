using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Windows.Media;
using System.Windows.Controls;

namespace QuickLauncher
{
    public partial class Settings : MetroWindow
    {
        public bool PasswordIsCorrect;

        public Settings()
        {
            InitializeComponent();

            //styles and texts
            this.Title = Localizer.GetLocalized("settings-title");

            txt_label1.Text = Localizer.GetLocalized("settings-sortorder");
            txt_label2.Text = Localizer.GetLocalized("settings-columns");
            txt_label3.Text = Localizer.GetLocalized("settings-groups");
            txt_label4.Text = Localizer.GetLocalized("settings-url");
            txt_label5.Text = Localizer.GetLocalized("settings-size");
            txt_label6.Text = Localizer.GetLocalized("settings-separator");

            //slider parameters
            slider_columns.Minimum = MainWindow.Settings.MinColumns;
            slider_columns.Maximum = MainWindow.Settings.MaxColumns;
            slider_groups.Minimum = MainWindow.Settings.MinGroups;
            slider_groups.Maximum = MainWindow.Settings.MaxGroups;
            slider_size.Minimum = MainWindow.Settings.MinButtonSize;
            slider_size.Maximum = MainWindow.Settings.MaxButtonSize;
            slider_separator.Minimum = MainWindow.Settings.MinGroupSeparatorSize;
            slider_separator.Maximum = MainWindow.Settings.MaxGroupSeparatorSize;

            //combobox values
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.Name, name = Localizer.GetLocalized("settings-sorting-name") });
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.MostUsed, name = Localizer.GetLocalized("settings-sorting-mostused") });
            combobox_sortorder.Items.Add(new Classes.Setting.ComboboxSortorder() { sortorder = Classes.Enums.SortOrder.SortOrder, name = Localizer.GetLocalized("settings-sorting-sortorder") });

            //set the values
            combobox_sortorder.SelectedValue = MainWindow.Settings.sortorder;
            textbox_materialurl.Text = MainWindow.Settings.iconpack_url;
            slider_columns.Value = MainWindow.Settings.columns;
            slider_groups.Value = MainWindow.Settings.groups;
            slider_size.Value = MainWindow.Settings.button_size;
            slider_separator.Value = MainWindow.Settings.group_spacing;

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                textbox_materialurl.Style = (Style)FindResource("TextBox_Normal_DarkMode");
                combobox_sortorder.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#acb6d2");

                txt_label1.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label2.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label3.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label4.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label5.Foreground = Classes.ResourceController.BrushDarkModeText;
                txt_label6.Foreground = Classes.ResourceController.BrushDarkModeText;

                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("settings-save"));
                button_validate.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Validate, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("settings-validate"));
            }
            else
            {
                button_save.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Save, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("settings-save"));
                button_validate.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Validate, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("settings-validate"));
            }
        }


        /// <summary>
        /// Clears the combobox focus to set the border to the default style again
        /// </summary>
        private void ClearComboBoxFocus()
        {
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(combobox_sortorder), null);
            Keyboard.ClearFocus();
        }


        /// <summary>
        /// Saves the settings and closes the window
        /// </summary>
        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Settings.columns = Convert.ToInt32(slider_columns.Value);
            MainWindow.Settings.groups = Convert.ToInt32(slider_groups.Value);
            MainWindow.Settings.button_size = Convert.ToInt32(slider_size.Value);
            MainWindow.Settings.group_spacing = Convert.ToInt32(slider_separator.Value);
            MainWindow.Settings.sortorder = ((Classes.Setting.ComboboxSortorder)combobox_sortorder.SelectedItem).sortorder;
            MainWindow.Settings.iconpack_url = textbox_materialurl.Text.Trim();

            if (textbox_materialurl.Text.Contains("://"))
                MainWindow.Settings.iconpack_url = textbox_materialurl.Text.Trim();
            else
                MainWindow.Settings.iconpack_url = null;

            //save and sort
            MainWindow.Settings.Save();
            MainWindow.Settings.Sort();

            //rebuild the shortcuts
            ((MainWindow)Application.Current.MainWindow).InitAppStuff(true);

            this.DialogResult = true;
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (!textbox_materialurl.IsFocused)
            {
                ClearComboBoxFocus();
            }

            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Escape)
            {
                this.DialogResult = true;
            }
        }


        /// <summary>
        /// Combobox selection changed event handler
        /// </summary>
        private void combobox_sortorder_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ClearComboBoxFocus();
        }


        /// <summary>
        /// Window left mouse up event handler
        /// </summary>
        private void MetroWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ClearComboBoxFocus();
        }


        /// <summary>
        /// Check all the icon if the shortcut is still correct. If not make red
        /// </summary>
        private void button_validate_Click(object sender, RoutedEventArgs e)
        {
            var result = Classes.Setting.ValidateShortcuts();

            //paint the incorrect icons red
            foreach (var item in result.shorcuts_with_error)
            {
                //item.color_button = ResourceController.BrushRed.Color.ToString();
                var icon = ((MainWindow)Application.Current.MainWindow).grid_main.FindChild<Button>("button_" + item);

                icon.Background = Classes.ResourceController.BrushRed;
            }

            //show result
            MessageBox.Show(result.message);            
        }
    }
}
