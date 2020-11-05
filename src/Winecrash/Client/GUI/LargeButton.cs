using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WEngine;
using WEngine.GUI;

namespace Winecrash.GUI
{
    public class LargeButton : GameButton
    {
        static LargeButton()
        {
            if(!Texture.Find("button"))
            {
                new Texture("assets/textures/gui/button.png");
            }
        }

        protected override void Creation()
        {
            base.Creation();

            Button.Background.Picture = Texture.Find("button");
        }
    }
}
