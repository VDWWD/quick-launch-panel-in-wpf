using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace QuickLauncher.Classes
{
    public class MaterialDesignIcons
    {
        /// <summary>
        /// Download the materialdesignicons master.zip from github.com. Then read the zip to get all the icon names and their path and store them on the disk in json format
        /// This is the only time the app will try to connect to the internet. Delete the json to trigger the download again
        /// </summary>
        public static void CreateIconList(Setting.Settings settings)
        {
            if (string.IsNullOrEmpty(settings.iconpack_url) || !settings.iconpack_url.Contains("://"))
                return;

            var list = new List<MaterialIcon>();

            using (var client = new WebClient())
            {
                //download de material icons from github
                var data = client.DownloadData(settings.iconpack_url);

                //read the downloaded zip
                using (var zip = new ZipArchive(new MemoryStream(data), ZipArchiveMode.Read))
                {
                    //get all the svg's from the correct directory. Skip 1 is needed to exclude the folder name itself
                    foreach (var entry in zip.Entries.Where(x => x.FullName.Contains("MaterialDesign-master/svg/")).Skip(1))
                    {
                        //the filename without extension
                        string name = Path.GetFileNameWithoutExtension(entry.FullName).ToLower();

                        using (var reader = new StreamReader(entry.Open()))
                        {
                            string svg = reader.ReadToEnd();

                            //try to get the path
                            var path = svg.Replace("'", "\"").Split(new[] { "path d=\"" }, StringSplitOptions.None).Last().Split('"').First();

                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(path))
                                continue;

                            //add the icon to the list
                            list.Add(new MaterialIcon()
                            {
                                name = name,
                                path = path
                            });
                        }
                    }
                }
            }

            //if the list is empty then do not save
            if (list.Count == 0)
                return;

            //serialize the list
            string json = JSONSerializer<List<MaterialIcon>>.Serialize(list);

            //write the file to disk
            File.WriteAllText(Globals.AppPathIconPackFile, json);

            //check for material icons in the shortcuts list
            settings.CheckIcons(list);
        }


        [DataContract]
        public class MaterialIcon
        {
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string path { get; set; }
        }
    }
}
