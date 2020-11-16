using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEngine;
using WEngine.GUI;
using Graphics = WEngine.Graphics;

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
            
            if (Input.IsPressing(Keys.W))
            {
                Time.TimeScale = Time.TimeScale * 0.1D;
            }
            if (Input.IsPressing(Keys.X))
            {
                Time.TimeScale = Time.TimeScale * 10D;
            }
            if (Input.IsPressing(Keys.C))
            {
                Time.TimeScale = 1.0D;
            }
                
            //screenshot
            if (Input.IsPressing(Keys.F2))
            {
                Task.Run(() =>
                {
                    Bitmap bmp = Graphics.Window.Screenshot();

                    int fileCount = System.IO.Directory.EnumerateFiles(Folders.UserData + "/Screenshots/", "*.png",
                        SearchOption.TopDirectoryOnly).Count();

                    bmp.Save(Folders.UserData + $"/Screenshots/Winecrash_Screenshot_{fileCount}.png");
                    bmp.Dispose();
                });
            }
        }
    }
}