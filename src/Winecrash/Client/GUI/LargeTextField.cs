using System.Runtime.CompilerServices;
using WEngine;
using WEngine.GUI;

namespace Winecrash.GUI
{
    public class LargeTextField : TextInputField
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
                this.EmptyText = Game.Language == null ? value : Game.Language.GetText(value, LocalizationArgs);
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
            base.Creation();
            
            this.Localization = "#enter_text";

            Game.OnLanguageChanged += (lang) => this.Localization = Localization;
            
            this.KeepRatio = true;
            this.Background.Picture = Texture.GetOrCreate("assets/textures/gui/text_field.png");
            this.Label.MinAnchor = new Vector2D(0.02, 0.12);
            this.Label.MaxAnchor = new Vector2D(0.98, 0.88);
            this.Label.Color = new Color256(1.0, 1.0, 1.0, 1.0);
            this.Label.AutoSize = true;
        }
    }
}