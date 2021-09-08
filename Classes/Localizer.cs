using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickLauncher
{
    public class Localizer
    {
        public static List<LanguageEntry> LocalizationData { get; set; }


        /// <summary>
        /// Get the correct localized text based on the key
        /// </summary>
        /// <param name="key">The key for the localized text</param>
        /// <returns>String</returns>
        public static string GetLocalized(string key)
        {
            if (LocalizationData == null)
                CreateLanguageData();
                
            var item = LocalizationData.Where(x => x.key == key).FirstOrDefault();

            //key not found
            if (item == null)
            {
                return "KEY-NOT-FOUND" + key;
            }

            string value;
            if (Classes.Globals.Language == Classes.Enums.Language.NL)
            {
                value = item.nl;
            }
            else if (Classes.Globals.Language == Classes.Enums.Language.DE)
            {
                value = item.de;
            }
            else
            {
                value = item.en;
            }

            if (value == null)
            {
                return "VALUE-NOT-FOUND";
            }
            else
            {
                return value;
            }
        }


        /// <summary>
        /// Creates a list with all the localized items. This could be modified to create a list from another internal or external source
        /// </summary>
        public static void CreateLanguageData()
        {
            LocalizationData = new List<LanguageEntry>()
            {
                new LanguageEntry()
                {
                    key = "mainwindow-error",
                    nl = "Kritieke fout!",
                    en = "Critical error!",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-closeapp",
                    nl = "Weet je zeker dat je {APPNAME} wilt afsluiten?",
                    en = "Are you sure you want to close {APPNAME}?",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-closing",
                    nl = "Afsluiten...",
                    en = "Closing...",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-close",
                    nl = "{APPNAME} afsluiten",
                    en = "Exit {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-restore",
                    nl = "{APPNAME} openen",
                    en = "Restore {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-minimize",
                    nl = "{APPNAME} minimaliseren",
                    en = "Minimize {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-maximize",
                    nl = "{APPNAME} maximaliseren",
                    en = "Maximize {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-about",
                    nl = "Over {APPNAME}",
                    en = "About {APPNAME}",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-ontop",
                    nl = "Bovenste venster aan/uit",
                    en = "Toggle always on top",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "about-version",
                    nl = "Versie",
                    en = "Version",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "about-ok",
                    nl = "OK",
                    en = "OK",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "mainwindow-cancel",
                    nl = "Annuleren",
                    en = "Cancel",
                    de = ""
                },                  
                

                //appspecifiek

                new LanguageEntry()
                {
                    key = "app-read-error",
                    nl = "Het lezen van de instellingen is mislukt.",
                    en = "The settings failed to load.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "app-write-error",
                    nl = "Het opslaan van de instellingen is mislukt.",
                    en = "The settings failed to save.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "app-error",
                    nl = $"{Classes.Globals.AppName} Fout",
                    en = $"{Classes.Globals.AppName} Error",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-iconpack-error",
                    nl = $"Het aanmaken van de Icon Pack is mislukt.",
                    en = $"There was an error creating the Icon Pack.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-tooltip-settings",
                    nl = string.Format("Instellingen wijzigen van {0}.", Classes.Globals.AppName),
                    en = string.Format("Change the {0} settings.", Classes.Globals.AppName),
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-tooltip-add",
                    nl = "Snelkoppeling toevoegen.",
                    en = "Add a shortcut.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-tooltip-edit",
                    nl = "Snelkoppelingen bewerken.",
                    en = "Edit shortcuts.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-title",
                    nl = "Instellingen",
                    en = "Settings",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sortorder",
                    nl = "Sorteervolgorde",
                    en = "Sort order",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-separator",
                    nl = "Groep scheidingshoogte",
                    en = "Group separator height",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-columns",
                    nl = "Aantal kolommone",
                    en = "Number of columns",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-groups",
                    nl = "Aantal groepen",
                    en = "Number of groups",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-size",
                    nl = "Button grootte",
                    en = "Button size",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-url",
                    nl = "MaterialIcons GitHub download URL",
                    en = "MaterialIcons GitHub download URL",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-mostused",
                    nl = "Meest gebruikt",
                    en = "Most used",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-name",
                    nl = "Naam",
                    en = "Name",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-save",
                    nl = "Opslaan",
                    en = "Save",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-validate",
                    nl = "Check",
                    en = "Check",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-checked-ok",
                    nl = "Alle snelkoppelingen zijn gecontroleerd. Er zijn geen problemen gevonden.",
                    en = "All shortcuts were checked. No problems were found.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-checked-error",
                    nl = "Er zijn {0} ontbrekende snelkoppelingen gevonden. Deze zijn rood gemarkeerd.",
                    en = "There are {0} missing shortcuts found. They are marked red.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "settings-sorting-sortorder",
                    nl = "Index nummer",
                    en = "Index number",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-save",
                    nl = "Opslaan",
                    en = "Save",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-delete",
                    nl = "Verwijder",
                    en = "Delete",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-new",
                    nl = "Nieuwe snelkoppeling toevoegen",
                    en = "Add New Shortcut",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-edit",
                    nl = "Bewerk snelkoppeling: ",
                    en = "Edit Shortcut: ",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-name",
                    nl = "Naam",
                    en = "Name",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-group",
                    nl = "Groep",
                    en = "Group",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-exec",
                    nl = "Executable pad",
                    en = "Executable path",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-args",
                    nl = "Extra argumenten",
                    en = "Extra arguments",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-icon",
                    nl = "Icoon path",
                    en = "Icon path",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-colorbutton",
                    nl = "Knop kleur",
                    en = "Button color",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-coloricon",
                    nl = "Icoon kleur",
                    en = "Icon color",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-index",
                    nl = "Index nummer",
                    en = "Index number",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-delete-confirm",
                    nl = "Weet je zeker dat je snelkoppeling \"{0}\" wil verwijderden?",
                    en = "Are you sure you want to delete the shortcut \"{0}\"?",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-delete-title",
                    nl = "Snelkoppeling verwijderen?",
                    en = "Remove shortcut?",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-tooltip-exec",
                    nl = "Selecteer een bestand van de schijf.",
                    en = "Select a file from disk.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-tooltip-icon",
                    nl = "Zoek een icoontje.",
                    en = "Search for an icon.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-tooltip-color",
                    nl = "Open de kleurenkiezer.",
                    en = "Open the color picker.",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "colorpicker-title",
                    nl = "Kleurenkiezer",
                    en = "Color Picker",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-date-added",
                    nl = "Datum toegevoegd:",
                    en = "Date added:",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-date-used",
                    nl = "Datum laatst gebruikt:",
                    en = "Date last used:",
                    de = ""
                },
                new LanguageEntry()
                {
                    key = "editshortcut-usage",
                    nl = "Aantal keer gebruikt:",
                    en = "Numer of times used:",
                    de = ""
                },
            };

            LocalizationData.ForEach(x => x.nl = x.nl.Replace("{APPNAME}", Classes.Globals.AppName));
            LocalizationData.ForEach(x => x.en = x.en.Replace("{APPNAME}", Classes.Globals.AppName));
        }


        public class LanguageEntry
        {
            public string key { get; set; }
            public string nl { get; set; }
            public string en { get; set; }
            public string de { get; set; }
        }
    }
}