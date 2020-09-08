using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Game
{
    public class GameConfig : Config
    {
        [Synchronize]
        public string Language { get; set; } = "English (United Kingdom)";

        [JsonIgnore]
        public Language UsedLanguage
        {
            get
            {
                return Winecrash.Game.Language.Languages.FirstOrDefault(l => l.Name == Language);
            }

            set
            {
                if (value == null)
                {
                    Language = null;
                }

                else
                {
                    Language = value.Name;
                }

                try
                {
                    Game.InvokeLanguageChanged(value);
                }
                catch(Exception e) { }

                FireConfigChanged();
            }
        }

        public GameConfig(string path) : base(path) { }
    }
}
