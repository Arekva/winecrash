using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WEngine;
using WEngine.GUI;
using Winecrash.Entities;
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
                PlayerEntity.PlayerHandWobject.Enabled = !PlayerEntity.PlayerHandWobject;
            }
            
            if (Input.IsPressing(Keys.W))
            {
                Time.Scale = Time.Scale * 0.1D;
            }
            if (Input.IsPressing(Keys.X))
            {
                Time.Scale = Time.Scale * 10D;
            }
            if (Input.IsPressing(Keys.C))
            {
                Time.Scale = 1.0D;
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