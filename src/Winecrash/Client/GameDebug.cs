using WEngine;
using WEngine.GUI;

namespace Winecrash.Client
{
    public class GameDebug : Module
    {
        protected override void Update()
        {
            if (Input.IsPressing(Keys.F3))
            {
                Canvas.Main.UICamera.Enabled = !Canvas.Main.UICamera.Enabled;
            }
        }
    }
}