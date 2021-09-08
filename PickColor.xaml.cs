using System;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QuickLauncher
{
    public partial class PickColor : MetroWindow
    {
        public bool ForButtonColor { get; set; }

        public PickColor(bool for_button_color, Color color)
        {
            InitializeComponent();
            ForButtonColor = for_button_color;

            this.Title = Localizer.GetLocalized("colorpicker-title");

            //darkmode detected then change colors
            if (Classes.Globals.IsDarkMode())
            {
                this.Background = Classes.ResourceController.BrushDarkModeBackground;

                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushBlack, Localizer.GetLocalized("about-ok"));
            }
            else
            {
                button_ok.Content = Classes.IconController.GetButtonIcon(Classes.Enums.Icon.Check, Classes.ResourceController.BrushWhite, Localizer.GetLocalized("about-ok"));
            }

            //set the color
            color_picker.SelectedColor = color;
        }


        /// <summary>
        /// Closes the window and sets the color to the correct textboxes
        /// </summary>
        private void button_ok_Click(object sender, RoutedEventArgs e)
        {
            var owner = ((EditItem)Owner);
            string hexcolor = string.Format("#{0:X2}{1:X2}{2:X2}", color_picker.SelectedColor.R, color_picker.SelectedColor.G, color_picker.SelectedColor.B);

            if (ForButtonColor)
            {
                owner.textbox_color_button.Text = hexcolor;
            }
            else
            {
                owner.textbox_color_icon.Text = hexcolor;
            }

            this.DialogResult = true;
        }


        /// <summary>
        /// Handles the keypresses in the window to close it on escape or enter
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                button_ok_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                this.DialogResult = true;
            }
        }
    }
}
