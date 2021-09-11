using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuickLauncher.Classes
{
    // https://materialdesignicons.com

    public class IconController
    {
        /// <summary>
        /// Find an icon and return its path
        /// </summary>
        /// <param name="icon">The icon name in Enum</param>
        /// <param name="color">The color for the icon as Brush</param>
        /// <returns>Path</returns>
        public static Path GetIcon(Enums.Icon icon, Brush color)
        {
            var item = IconList().Where(x => x.icon == icon).FirstOrDefault();
            return item.GetPath(color);
        }


        /// <summary>
        /// Get the button icon adn textblock
        /// </summary>
        /// <param name="icon">The icon name in Enum</param>
        /// <param name="color">The color for the icon as Brush</param>
        /// <param name="text">The text in the button</param>
        /// <param name="opacity">Transparency of the icon, only used in the MainWindow icon at the bottom</param>
        /// <returns>A dockpanel with text an icon for the button</returns>
        public static DockPanel GetButtonIcon(Enums.Icon icon, Brush color, string text, double opacity = 1)
        {
            var item = IconList().Where(x => x.icon == icon).FirstOrDefault();

            int size = 24;
            int width = 80;
            var margin = new Thickness(3, -1, 6, 0);
            var path = item.GetPath(color);

            //set opacity
            if (opacity < 1)
            {
                path.Opacity = opacity;
            }

            //no text then no margin
            if (string.IsNullOrEmpty(text))
            {
                margin = new Thickness(0, 0, 0, 0);
                width = size;
            }

            var canvas = new Canvas()
            {
                Width = size,
                Height = size
            };

            var viewBox = new Viewbox()
            {
                Width = size,
                Height = size,
                Margin = margin
            };

            var dockpanel = new DockPanel()
            {
                Width = width,
            };

            canvas.Children.Add(path);
            viewBox.Child = canvas;
            dockpanel.Children.Add(viewBox);

            //if there is text, add a textblock
            if (!string.IsNullOrEmpty(text))
            {
                var textblock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = color,
                    Style = (Style)Application.Current.FindResource("ButtonText"),
                    Text = text
                };

                dockpanel.Children.Add(textblock);
            }

            return dockpanel;
        }


        /// <summary>
        /// A list with all the available icons and their path as string
        /// </summary>
        /// <returns>List with icons</returns>
        public static List<ButtonIcon> IconList()
        {
            return new List<ButtonIcon>()
            {
                new ButtonIcon() {
                    icon = Enums.Icon.Check,
                    path = "M9,20.42L2.79,14.21L5.62,11.38L9,14.77L18.88,4.88L21.71,7.71L9,20.42Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Maximize,
                    path = "M4,4H20V20H4V4M6,8V18H18V8H6Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Validate,
                    path = "M9.47 9.65L8.06 11.07L11 14L16.19 8.82L14.78 7.4L11 11.18M17 3H7C5.9 3 5 3.9 5 5L5 21L12 18L19 21V5C19 3.9 18.1 3 17 3M17 18L12 15.82L7 18V5H17Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Save,
                    path = "M5,3A2,2 0 0,0 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V5.5L18.5,3H17V9A1,1 0 0,1 16,10H8A1,1 0 0,1 7,9V3H5M12,4V9H15V4H12M7,12H17A1,1 0 0,1 18,13V19H6V13A1,1 0 0,1 7,12Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Close,
                    path = "M20 6.91L17.09 4L12 9.09L6.91 4L4 6.91L9.09 12L4 17.09L6.91 20L12 14.91L17.09 20L20 17.09L14.91 12L20 6.91Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Settings,
                    path = "M12,8A4,4 0 0,1 16,12A4,4 0 0,1 12,16A4,4 0 0,1 8,12A4,4 0 0,1 12,8M12,10A2,2 0 0,0 10,12A2,2 0 0,0 12,14A2,2 0 0,0 14,12A2,2 0 0,0 12,10M10,22C9.75,22 9.54,21.82 9.5,21.58L9.13,18.93C8.5,18.68 7.96,18.34 7.44,17.94L4.95,18.95C4.73,19.03 4.46,18.95 4.34,18.73L2.34,15.27C2.21,15.05 2.27,14.78 2.46,14.63L4.57,12.97L4.5,12L4.57,11L2.46,9.37C2.27,9.22 2.21,8.95 2.34,8.73L4.34,5.27C4.46,5.05 4.73,4.96 4.95,5.05L7.44,6.05C7.96,5.66 8.5,5.32 9.13,5.07L9.5,2.42C9.54,2.18 9.75,2 10,2H14C14.25,2 14.46,2.18 14.5,2.42L14.87,5.07C15.5,5.32 16.04,5.66 16.56,6.05L19.05,5.05C19.27,4.96 19.54,5.05 19.66,5.27L21.66,8.73C21.79,8.95 21.73,9.22 21.54,9.37L19.43,11L19.5,12L19.43,13L21.54,14.63C21.73,14.78 21.79,15.05 21.66,15.27L19.66,18.73C19.54,18.95 19.27,19.04 19.05,18.95L16.56,17.95C16.04,18.34 15.5,18.68 14.87,18.93L14.5,21.58C14.46,21.82 14.25,22 14,22H10M11.25,4L10.88,6.61C9.68,6.86 8.62,7.5 7.85,8.39L5.44,7.35L4.69,8.65L6.8,10.2C6.4,11.37 6.4,12.64 6.8,13.8L4.68,15.36L5.43,16.66L7.86,15.62C8.63,16.5 9.68,17.14 10.87,17.38L11.24,20H12.76L13.13,17.39C14.32,17.14 15.37,16.5 16.14,15.62L18.57,16.66L19.32,15.36L17.2,13.81C17.6,12.64 17.6,11.37 17.2,10.2L19.31,8.65L18.56,7.35L16.15,8.39C15.38,7.5 14.32,6.86 13.12,6.62L12.75,4H11.25Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.About,
                    path = "M11,18H13V16H11V18M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,6A4,4 0 0,0 8,10H10A2,2 0 0,1 12,8A2,2 0 0,1 14,10C14,12 11,11.75 11,15H13C13,12.75 16,12.5 16,10A4,4 0 0,0 12,6Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Pin,
                    path = "M8,6.2V4H7V2H17V4H16V12L18,14V16H17.8L14,12.2V4H10V8.2L8,6.2M20,20.7L18.7,22L12.8,16.1V22H11.2V16H6V14L8,12V11.3L2,5.3L3.3,4L20,20.7M8.8,14H10.6L9.7,13.1L8.8,14Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Unpin,
                    path = "M16,12V4H17V2H7V4H8V12L6,14V16H11.2V22H12.8V16H18V14L16,12M8.8,14L10,12.8V4H14V12.8L15.2,14H8.8Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Add,
                    path = "M20 14H14V20H10V14H4V10H10V4H14V10H20V14Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Edit,
                    path = "M2,6V8H14V6H2M2,10V12H14V10H2M20.04,10.13C19.9,10.13 19.76,10.19 19.65,10.3L18.65,11.3L20.7,13.35L21.7,12.35C21.92,12.14 21.92,11.79 21.7,11.58L20.42,10.3C20.31,10.19 20.18,10.13 20.04,10.13M18.07,11.88L12,17.94V20H14.06L20.12,13.93L18.07,11.88M2,14V16H10V14H2Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Browse,
                    path = "M16.5,12C19,12 21,14 21,16.5C21,17.38 20.75,18.21 20.31,18.9L23.39,22L22,23.39L18.88,20.32C18.19,20.75 17.37,21 16.5,21C14,21 12,19 12,16.5C12,14 14,12 16.5,12M16.5,14A2.5,2.5 0 0,0 14,16.5A2.5,2.5 0 0,0 16.5,19A2.5,2.5 0 0,0 19,16.5A2.5,2.5 0 0,0 16.5,14M19,8H3V18H10.17C10.34,18.72 10.63,19.39 11,20H3C1.89,20 1,19.1 1,18V6C1,4.89 1.89,4 3,4H9L11,6H19A2,2 0 0,1 21,8V11.81C20.42,11.26 19.75,10.81 19,10.5V8Z"
                },
                new ButtonIcon() {
                    icon = Enums.Icon.Delete,
                    path = "M9,3V4H4V6H5V19A2,2 0 0,0 7,21H17A2,2 0 0,0 19,19V6H20V4H15V3H9M7,6H17V19H7V6M9,8V17H11V8H9M13,8V17H15V8H13Z"
                },
                new ButtonIcon()
                {
                    icon = Enums.Icon.Colorpicker,
                    path = "M6.92,19L5,17.08L13.06,9L15,10.94M20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L13.84,6.41L11.91,4.5L10.5,5.91L11.92,7.33L3,16.25V21H7.75L16.67,12.08L18.09,13.5L19.5,12.09L17.58,10.17L20.7,7.05C21.1,6.65 21.1,6 20.71,5.63Z"
                },
                new ButtonIcon()
                {
                    icon = Enums.Icon.Questionmark,
                    path = "M10,19H13V22H10V19M12,2C17.35,2.22 19.68,7.62 16.5,11.67C15.67,12.67 14.33,13.33 13.67,14.17C13,15 13,16 13,17H10C10,15.33 10,13.92 10.67,12.92C11.33,11.92 12.67,11.33 13.5,10.67C15.92,8.43 15.32,5.26 12,5A3,3 0 0,0 9,8H6A6,6 0 0,1 12,2Z"
                }
            };
        }


        public class ButtonIcon
        {
            public Enums.Icon icon { get; set; }
            public string path { get; set; }

            public Path GetPath(Brush brush)
            {
                return new Path()
                {
                    Fill = brush,
                    Data = Geometry.Parse(path)
                };
            }
        }



        public class FaviconDoubleClickCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                ((MainWindow)Application.Current.MainWindow).NormalWindow();
            }
        }
    }
}
