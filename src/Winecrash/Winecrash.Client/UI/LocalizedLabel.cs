using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine.GUI;

namespace Winecrash.Game.UI
{
    public class LocalizedLabel : Label
    {
        private string _Localization;

        public string Localization
        {
            get
            {
                return _Localization;
            }

            set
            {
                this._Localization = value;
                this.Text = Game.Language.GetText(value, LocalizationArgs);
            }
        }

        private object[] _LocalizationArgs;
        public object[] LocalizationArgs
        {
            get
            {
                return _LocalizationArgs;
            }

            set
            {
                _LocalizationArgs = value;
                Localization = Localization;
            }
        }

        protected override void Creation()
        {
            this.Localization = "#error_undefined_text";

            Game.OnLanguageChanged += (lang) => this.Localization = Localization;

            base.Creation();      
        }
    }
}
