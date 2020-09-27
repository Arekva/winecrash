using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;

namespace Winecrash
{
    public delegate void LanguageChangeDelegate(Language language);
    
    public static class Game
    {
        public static WEngine.Version Version { get; } = new WEngine.Version(0, 0, 1, "Alpha");

        /// <summary>
        /// Triggered when a new game language is set. Carries the new language object with it.
        /// </summary>
        public static event LanguageChangeDelegate OnLanguageChanged;

        public static GameConfig Configuration { get; } = new GameConfig("settings.json");

        public static string LastAddress
        {
            get
            {
                return Configuration.LastEnteredAddress;
            }
            set
            {
                Configuration.LastEnteredAddress = value;
            }
        }
        

        public const string DefaultLanguage = "English (United Kingdom)";

        /// <summary>
        /// The language used by the game. Defaults to <see cref="DefaultLanguage"/>
        /// </summary>
        public static Language Language
        {
            get
            {
                return Language.Languages.FirstOrDefault(l => l.Name == Configuration.Language);
            }

            set
            {
                if (value == null)
                {
                    Configuration.Language = DefaultLanguage;
                }

                else
                {
                    Configuration.Language = value.Name;
                }

                Game.InvokeLanguageChanged(value);
            }
        }

        static Game()
        {
            LoadLanguagesInFolder("assets/languages/");
        }

        internal static void InvokeLanguageChanged(Language lang)
        {
            OnLanguageChanged?.Invoke(lang);
        }

        internal static List<Language> LoadLanguagesInFolder(string path)
        {
            List<Language> langs = new List<Language>();
            foreach(string str in Directory.GetFiles(path, "*.json"))
            {
                try
                {
                    langs.Add(new Language(str));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return langs;
        }
    }
}
