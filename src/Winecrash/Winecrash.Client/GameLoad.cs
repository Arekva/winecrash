using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game
{
    public static class GameLoad
    {
        public static WObject LoadScene;
        public static async void StartLoad()
        {
            await DisplayScene();

            await Task.Delay(1000);

            DeleteScene();
        }

        public static Task DisplayScene()
        {
            return Task.Run(() =>
            {
                Camera.Main.ClearColor = new Color256(1.0D, 1.0D, 1.0D, 1.0D);

                LoadScene = new WObject("LoadScene");
                LoadScene.Parent = Canvas.Main.WObject;
                Image bg = LoadScene.AddModule<Image>();
                Image logo = LoadScene.AddModule<Image>();
                logo.Picture = new Texture("assets/textures/logo.png");
                logo.KeepRatio = true;
                logo.MinAnchor = new Vector2F(0.3F, 0.6F);
                logo.MaxAnchor = new Vector2F(0.7F, 0.8F);
            });
        }
        public static void DeleteScene()
        {
            LoadScene.Delete();
        }
    }
}
