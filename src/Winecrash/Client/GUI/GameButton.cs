using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using WEngine.GUI;

namespace Winecrash.GUI
{
    public class GameButton : Module
    {
        public Button Button { get; private set; }
        public LocalizedLabel Label { get; private set; } 
        public static Color256 LabelColor { get; } = new Color256(0.95, 0.95, 0.95, 1.0);
        public static Color256 LabelHoverColor { get; } = new Color256(249 / 255.0D, 255 / 255.0D, 191 / 255.0D, 1.0D);
        public static Color256 ButtonColor { get; } = new Color256(1.0D, 1.0D, 1.0D, 1.0D);
        public static Color256 ButtonHoverColor { get; } = new Color256(204 / 255.0D, 252 / 255.0D, 255 / 255.0D, 255 / 255.0D);
        public static Color256 ButtonLockColor { get; } = Color256.DarkGray;
        public static Color256 LabelLockColor { get; } = new Color256(0.65, 0.65, 0.65, 1.0);

        public static Sound ClickSound { get; set; }
        //static int count = 0;
        protected override void Creation()
        {
            Button = this.WObject.AddModule<Button>();

            LocalizedLabel label = Button.Label.WObject.AddModule<LocalizedLabel>();

            Button.Label.Delete();

            Button.Label = Label = label;


            Button.Label.Color = LabelColor;
            Button.HoverColor = ButtonHoverColor;
            Button.IdleColor = ButtonColor;
            Button.OnHover += () => Button.Label.Color = LabelHoverColor;
            Button.OnUnhover += () => Button.Label.Color = LabelColor;
            Button.OnLock += () => Button.Label.Color = LabelLockColor;
            Button.OnUnlock += () => Button.Label.Color = LabelColor;

            
            Button.OnClick += () =>
            {
                if (!ClickSound) ClickSound = Sound.Find("button_click");
                ClickSound.Play();
            };

            Button.KeepRatio = true;
            Label.AutoSize = true;
            Label.Aligns = TextAligns.Middle;
            Label.MinAnchor = new Vector2F(0.0F, 0.12F);
            Label.MaxAnchor = new Vector2F(1.0F, 0.88F);
        }
    }
}
