using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game.UI
{
    public class GameButton : Module
    {
        public Button Button { get; private set; }
        public static Color256 LabelColor { get; } = new Color256(0.95, 0.95, 0.95, 1.0);
        public static Color256 LabelHoverColor { get; } = new Color256(249 / 255.0D, 255 / 255.0D, 191 / 255.0D, 1.0D);
        public static Color256 ButtonColor { get; } = new Color256(1.0D, 1.0D, 1.0D, 1.0D);
        public static Color256 ButtonHoverColor { get; } = new Color256(204 / 255.0D, 252 / 255.0D, 255 / 255.0D, 255 / 255.0D);
        public static Color256 ButtonLockColor { get; } = Color256.DarkGray;
        public static Color256 LabelLockColor { get; } = new Color256(0.65, 0.65, 0.65, 1.0);

        protected override void Creation()
        {
            Button = this.WObject.AddModule<Button>();

            Button.Label.Color = LabelColor;
            Button.HoverColor = ButtonHoverColor;
            Button.IdleColor = ButtonColor;
            Button.OnHover += () => Button.Label.Color = LabelHoverColor;
            Button.OnUnhover += () => Button.Label.Color = LabelColor;
            Button.OnLock += () => Button.Label.Color = LabelLockColor;
            Button.OnUnlock += () => Button.Label.Color = LabelColor;

            Button.KeepRatio = true;
            Button.Label.AutoSize = true;
            Button.Label.Aligns = TextAligns.Middle;
            Button.Label.MinAnchor = new Vector2F(0.0F, 0.12F);
            Button.Label.MaxAnchor = new Vector2F(1.0F, 0.88F);
        }
    }
}
