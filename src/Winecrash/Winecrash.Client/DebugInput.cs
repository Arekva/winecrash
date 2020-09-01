using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game
{
    class DebugInput : Module
    {
        public Image img;

        protected override void Creation()
        {
            img = this.WObject.AddModule<Image>();

            img.Picture = new Texture("debug.png");
        }
        protected override void LateUpdate()
        {
            float size = 5.0F;
            
            Vector2I mp = Input.MousePosition;

            float mpScreenRight = (float)WMath.Remap(mp.X, -Canvas.Main.Extents.X, Canvas.Main.Extents.X, 0.0F, Canvas.Main.Extents.X * 4.0D);
            float mpScreenTop = (float)WMath.Remap(mp.Y, -Canvas.Main.Extents.Y, Canvas.Main.Extents.Y, 0.0F, Canvas.Main.Extents.Y * 4.0D);

            img.Right = mpScreenRight - size;
            img.Left = (Canvas.Main.Extents.X * 4.0F) - mpScreenRight - size;

            img.Bottom = mpScreenTop - size;
            img.Top = (Canvas.Main.Extents.Y * 4.0F) - mpScreenTop - size;
        }
    }
}
