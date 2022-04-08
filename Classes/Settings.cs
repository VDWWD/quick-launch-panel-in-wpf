using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace QuickLauncher.Classes
{
    public class Setting
    {
        /// <summary>
        /// Loads the settings from the file on the disk
        /// </summary>
        /// <returns>The settings class</returns>
        public static Settings Load()
        {
            var settings = new Settings();

            //check if there is a settings file
            if (File.Exists(Globals.AppPathSettingsFile))
            {
                try
                {
                    using (var stream = File.OpenRead(Globals.AppPathSettingsFile))
                    using (var sr = new StreamReader(stream))
                    {
                        string xml = sr.ReadToEnd();
                        var serializer = new XmlSerializer(typeof(Settings));

                        var rdr = new StringReader(xml);
                        settings = (Settings)serializer.Deserialize(rdr);
                    }
                }
                catch
                {
                    //if incorrect xml then save again
                    settings.Save();

                    MessageBox.Show(Localizer.GetLocalized("app-read-error"), Localizer.GetLocalized("app-error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                //create a new config file if none exists
                settings.Save();
            }

            //does the icon pack exist, if not download
            if (!File.Exists(Globals.AppPathIconPackFile))
            {
                try
                {
                    Task.Run(() => MaterialDesignIcons.CreateIconList(settings));
                }
                catch
                {
                }
            }
            else
            {
                //check if there are svg or material icons
                settings.CheckIcons(null);
            }

            //sort the shortcuts
            settings.Sort();

            //add an index to the shortcuts
            int count = 1;
            settings.shortcuts.ForEach(x => x.id = count++);

            //for testing
            //settings.Test();

            return settings;
        }


        /// <summary>
        /// Check all the icon if the shortcut is still correct. If not make red
        /// </summary>
        /// <returns>ValidateResult class with message and list of icons with errors</returns>
        public static ValidateResult ValidateShortcuts()
        {
            var result = new ValidateResult()
            {
                shorcuts_with_error = new List<int>()
            };

            //loop all the shortcuts
            foreach (var item in MainWindow.Settings.shortcuts)
            {
                if (item.executable_path.Contains("http") && item.executable_path.Contains("://"))
                {
                    //check if the url is correct
                    using (var client = new WebClient())
                    {
                        try
                        {
                            client.DownloadString(item.executable_path);
                        }
                        catch
                        {
                            result.shorcuts_with_error.Add(item.id);
                        }
                    }
                }
                else if (!IsValidEmail(item.executable_path))
                {
                    //check if the file exits
                    if (!File.Exists(item.executable_path))
                    {
                        result.shorcuts_with_error.Add(item.id);
                    }
                }
            }

            //return the correct error message
            if (result.shorcuts_with_error.Count() == 0)
            {
                result.message = Localizer.GetLocalized("settings-checked-ok");
            }
            else
            {
                result.message = string.Format(Localizer.GetLocalized("settings-checked-error"), result.shorcuts_with_error.Count());
            }

            return result;
        }


        /// <summary>
        /// Check if a string is a valid email address
        /// </summary>
        /// <param name="email">The supposed email address</param>
        /// <returns>Boolean</returns>
        public static bool IsValidEmail(string email)
        {
            if (!string.IsNullOrEmpty(email) && new EmailAddressAttribute().IsValid(email))
                return true;
            else
                return false;
        }


        [XmlRoot(ElementName = "settings")]
        public class Settings
        {
            [XmlIgnoreAttribute]
            public int MinColumns = 5;
            [XmlIgnoreAttribute]
            public int MaxColumns = 20;
            [XmlIgnoreAttribute]
            public int MinGroups = 1;
            [XmlIgnoreAttribute]
            public int MaxGroups = 10;
            [XmlIgnoreAttribute]
            public int MinButtonSize = 36;
            [XmlIgnoreAttribute]
            public int MaxButtonSize = 256;
            [XmlIgnoreAttribute]
            public int MinGroupSeparatorSize = 20;
            [XmlIgnoreAttribute]
            public int MaxGroupSeparatorSize = 100;

            private int _columns;
            private int _groups;
            private int _button_size;
            private int _group_spacing;

            [XmlArrayItem("shortcut")]
            public List<ShortCut> shortcuts { get; set; }
            public bool always_on_top { get; set; }
            public Enums.SortOrder sortorder { get; set; }
            public string iconpack_url { get; set; }
            public int columns
            {
                get
                {
                    if (_columns < MinColumns)
                        _columns = MinColumns;
                    else if (_columns > MaxColumns)
                        _columns = MaxColumns;

                    return _columns;
                }
                set
                {
                    _columns = value;
                }
            }
            public int groups
            {
                get
                {
                    if (_groups < MinGroups)
                        _groups = MinGroups;
                    else if (_groups > MaxGroups)
                        _groups = MaxGroups;

                    return _groups;
                }
                set
                {
                    _groups = value;
                }
            }
            public int button_size
            {
                get
                {
                    if (_button_size < MinButtonSize)
                        _button_size = MinButtonSize;
                    else if (_button_size > MaxButtonSize)
                        _button_size = MaxButtonSize;

                    return _button_size;
                }
                set
                {
                    _button_size = value;
                }
            }
            public int group_spacing
            {
                get
                {
                    if (_group_spacing < MinGroupSeparatorSize)
                        _group_spacing = MinGroupSeparatorSize;
                    else if (_group_spacing > MaxGroupSeparatorSize)
                        _group_spacing = MaxGroupSeparatorSize;

                    return _group_spacing;
                }
                set
                {
                    _group_spacing = value;
                }
            }

            public Settings()
            {
                _columns = 6;
                _groups = 3;
                _button_size = 36;
                _group_spacing = 20;
                shortcuts = new List<ShortCut>();
                always_on_top = true;
                iconpack_url = "https://github.com/Templarian/MaterialDesign/archive/refs/heads/master.zip";
            }


            public void Save()
            {
                try
                {
                    using (var writer = new StreamWriter(Globals.AppPathSettingsFile))
                    using (var sw = new StringWriter())
                    {
                        var serializer = new XmlSerializer(this.GetType());
                        serializer.Serialize(sw, this);

                        writer.Write(sw.ToString());
                        writer.Flush();
                    }
                }
                catch
                {
                    MessageBox.Show(Localizer.GetLocalized("app-write-error"), Localizer.GetLocalized("app-error"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


            public void Sort()
            {
                if (sortorder == Enums.SortOrder.SortIndex)
                {
                    shortcuts = shortcuts.OrderBy(x => x.index_int).ThenBy(x => x.name).ToList();
                }
                else if (sortorder == Enums.SortOrder.MostUsed)
                {
                    shortcuts = shortcuts.OrderByDescending(x => x.clicks_int).ThenBy(x => x.name).ToList();
                }
                else
                {
                    shortcuts = shortcuts.OrderBy(x => x.name).ThenBy(x => x.index_int).ToList();
                }

                //make sure the shortcust atleast have a name and executable_path
                shortcuts = shortcuts.Where(x => !string.IsNullOrEmpty(x.executable_path) && !string.IsNullOrEmpty(x.name)).ToList();
            }


            //check if an icon is perhaps a svg or material icons and get the path. Then save again so this slow process is not needed on evert startup
            public void CheckIcons(List<MaterialDesignIcons.MaterialIcon> list)
            {
                bool changed = false;

                //if the list is null the try to load the iconpack and deserialize
                if (list == null)
                {
                    try
                    {
                        list = JSONSerializer<List<MaterialDesignIcons.MaterialIcon>>.DeSerialize(File.ReadAllText(Globals.AppPathIconPackFile));
                    }
                    catch
                    {
                    }
                }

                foreach (var item in shortcuts)
                {
                    if (string.IsNullOrEmpty(item.icon))
                        continue;

                    try
                    {
                        //check if the icon string is an svg and try to get the path
                        if (item.icon.ToLower().Contains("svg") && item.icon.ToLower().Contains("d="))
                        {
                            //check if the icon string is an svg and try to get the path

                            try
                            {
                                item.icon = item.icon.Replace("'", "\"").Replace("\\", "").Split(new[] { "d=\"" }, StringSplitOptions.None)[1].Split('"').First();
                                changed = true;
                            }
                            catch
                            {
                                item.icon = "";
                            }
                        }
                        //check if the icon string is a material icon
                        else if (!(item.icon.Contains(".") || item.icon.Contains(",") || item.icon.Contains(" ")) && list != null)
                        {
                            var materialicon = list.Where(x => x.name.ToLower() == item.icon.ToLower().Replace(".svg", "")).FirstOrDefault();

                            //icon found then store
                            if (materialicon != null)
                            {
                                item.icon = materialicon.path;
                                changed = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                //if changed save again
                if (changed)
                {
                    Save();
                }
            }


            //for testing add random icons
            public void Test()
            {
                shortcuts = new List<ShortCut>();
                var rnd = new Random();
                var list = JSONSerializer<List<MaterialDesignIcons.MaterialIcon>>.DeSerialize(File.ReadAllText(Globals.AppPathIconPackFile));
                var colors_button = new List<string>()
                {
                    "#ff686e",
                    "#ff966e",
                    "#ffff6e",
                    "#6eff7c",
                    "#6e99ff"
                };

                for (int i = 1; i <= groups; i++)
                {
                    for (int j = 0; j < rnd.Next(5, 18); j++)
                    {
                        //find a random icon
                        var icon = list[rnd.Next(0, list.Count)];

                        shortcuts.Add(new ShortCut()
                        {
                            id = i + j,
                            name = icon.name,
                            group = i.ToString(),
                            icon = icon.path,
                            executable_path = @"C:\Windows\system32\notepad.exe",
                            color_button = colors_button[rnd.Next(0, colors_button.Count)],
                            color_icon = j % 4 == 0 ? "#ffffff" : "#000000"
                        });
                    }
                }
            }
        }


        public class ShortCut
        {
            public string name { get; set; }
            public string executable_path { get; set; }
            public string executable_arguments { get; set; }
            public string icon { get; set; }
            private string _color_button;
            public string color_button
            {
                get
                {
                    if (string.IsNullOrEmpty(_color_button))
                        return "#687FBA";

                    return "#" + _color_button.Replace("#", "").ToUpper();
                }
                set
                {
                    _color_button = value;
                }
            }
            private string _color_icon;
            public string color_icon
            {
                get
                {
                    if (string.IsNullOrEmpty(_color_icon))
                        return "#000000";

                    return "#" + _color_icon.Replace("#", "").ToUpper();
                }
                set
                {
                    _color_icon = value;
                }
            }

            //made these string so a typo in the settings file will not cause a read error when loading the settings
            public string group { get; set; }
            public string index { get; set; }
            public string clicks { get; set; }
            public DateTime date_added { get; set; }
            public DateTime date_last_used { get; set; }

            [XmlIgnoreAttribute]
            public int group_int
            {
                get
                {
                    int _number = int.TryParse(group, out _number) ? _number : 0;

                    if (_number < 1)
                        _number = 1;
                    else if (_number > MainWindow.Settings.groups)
                        _number = MainWindow.Settings.groups;

                    group = _number.ToString();
                    return _number;
                }
            }
            [XmlIgnoreAttribute]
            public int index_int
            {
                get
                {
                    int _number = int.TryParse(index, out _number) ? _number : 0;

                    index = _number.ToString();
                    return _number;
                }
            }
            [XmlIgnoreAttribute]
            public int clicks_int
            {
                get
                {
                    int _number = int.TryParse(clicks, out _number) ? _number : 0;

                    clicks = _number.ToString();
                    return _number;
                }
                set
                {
                    clicks = value.ToString();
                }
            }
            [XmlIgnoreAttribute]
            public int id { get; set; }

            public ShortCut()
            {
                //need to set these variables to their defaults because they are strings, otherwise they would be missing in the settings xml
                index = "0";
                group = "1";
                clicks = "0";
                executable_arguments = "";
                date_added = DateTime.Now;
            }
        }


        public class ComboboxSortorder
        {
            public string name { get; set; }
            public Enums.SortOrder sortorder { get; set; }
        }


        public class ValidateResult
        {
            public string message { get; set; }
            public List<int> shorcuts_with_error { get; set; }
        }
    }
}
