using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;

using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Winecrash.Game
{
    /// <summary>
    /// Base language class describing a language and its translations
    /// </summary>
    public class Language : IEquatable<Language>
    {
        private static List<Language> _Languages = new List<Language>(16);
        /// <summary>
        /// All the languages loaded into the game.
        /// </summary>
        public static IReadOnlyList<Language> Languages
        {
            get
            {
                return _Languages.AsReadOnly();
            }
        }

        /// <summary>
        /// The name of the language into its own language, optinally with a region.
        /// <br>i.e. Français (France) for Metropolitan French and Français (Canada) for Canadian French</br>
        /// </summary>
        public string Name { get; private set; } = "Français (France)";

        /// <summary>
        /// All the localizations of the language
        /// </summary>
        internal Dictionary<string, string> Localizations { get; private set; }

        /// <summary>
        /// The icon of the language.
        /// </summary>
        public Texture Icon { get; set; } = Texture.Blank;

        /// <summary>
        /// JSON Deserializer constructor
        /// </summary>
        [JsonConstructor]
        internal Language(string Name, string Icon, Dictionary<string, string> Localizations)
        {
            this.Name = Name;
            this.Localizations = Localizations;

            this.Icon = new Texture(Icon, null, true);
        }
        /// <summary>
        /// Create a language from a file containing all the localizations.
        /// </summary>
        /// <param name="path">The emplacement of the language file.</param>
        public Language(string path)
        {
            if(File.Exists(path))
            {
                try
                {
                    Language lang = (Language)JsonConvert.DeserializeObject(
                        File.ReadAllText(path), typeof(Language), new JsonSerializerSettings() 
                        { Culture = System.Globalization.CultureInfo.InvariantCulture });

                    this.Name = lang.Name;
                    this.Localizations = lang.Localizations;
                    this.Icon = lang.Icon;

                    _Languages.Add(this);

                    lang.Name = null;
                    lang.Localizations = null;
                    lang.Icon = null;
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogError("Unable to find a language file at " + @path);
            }
        }

        /// <summary>
        /// Create a language from code.
        /// </summary>
        /// <param name="name">The name of the language.</param>
        /// <param name="icon">The icon texture representing the language.</param>
        /// <param name="localizations">The localization directionary.</param>
        public Language(string name, Texture icon, Dictionary<string, string> localizations)
        {
            this.Name = name;
            this.Icon = icon;
            this.Localizations = localizations;

            _Languages.Add(this);
        }

        /// <summary>
        /// Get the corresponding text from the language code.
        /// <br>i.e. "#block_dirt" in French would return "Terre", "Dirt" in English...</br>
        /// </summary>
        /// <param name="code">The text code.</param>
        /// <param name="args">The potential arguments to the text code. {0}, {1}, ..., {N} into the language strings.</param>
        /// <returns>The corresponding text into the language.<br>If none corresponding, returns the input code.</br></returns>
        public string GetText(string code, params object[] args)
        {
            if(Localizations.TryGetValue(code, out string text))
            {
                if (args == null)
                {
                    return text;
                }

                else
                {
                    return string.Format(text, args);
                }
            }

            else
            {
                return code;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is Language lang && Equals(lang);
        }
        public bool Equals(Language lang)
        {
            return lang.Name == this.Name;
        }
    }
}
