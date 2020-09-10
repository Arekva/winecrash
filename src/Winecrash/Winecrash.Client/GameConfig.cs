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
        private string _Language = "English (United Kingdom)";
        [Synchronize]
        public string Language
        {
            get
            {
                return this._Language;
            }

            set
            {
                this._Language = value;
                FireConfigChanged();
            }
        }

        

        public GameConfig(string path) : base(path) { }
    }
}
