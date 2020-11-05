using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEngine;

namespace Winecrash.GUI
{
    public class SmallButton : GameButton
    {
        static SmallButton()
        {
            if (!Texture.Find("short_button"))
            {
                new Texture("assets/textures/gui/short_button.png");
            }
        }

        protected override void Creation()
        {
            base.Creation();

            Button.Background.Picture = Texture.Find("short_button");
        }
    }
}
