using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash
{
    public class GameConfig : Config
    {
        private string _DisplayLanguage = Language.CultureToWinecrash(CultureInfo.InstalledUICulture);//"English (United Kingdom)";
        [Synchronize]
        public string DisplayLanguage
        {
            get
            {
                return this._DisplayLanguage;
            }

            set
            {
                this._DisplayLanguage = value;
                FireConfigChanged();
            }
        }

        private string _LastEnteredAddress = null;
        [Synchronize]
        public string LastEnteredAddress
        {
            get
            {
                return this._LastEnteredAddress;
            }

            set
            {
                this._LastEnteredAddress = value;
                FireConfigChanged();
            }
        }
        
        public GameConfig(string path) : base(path) { }
    }
}