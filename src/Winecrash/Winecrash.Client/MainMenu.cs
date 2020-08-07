using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game
{
    public static class MainMenu
    {
        private static WObject MenuWobject = null;
        private static void CreateMenu()
        {
            MenuWobject = new WObject("Main Menu");

            //Panel
            WObject bgPanel = new WObject("Background Panel") { Parent = MenuWobject };
            bgPanel.Parent = MenuWobject;
            //BG Image
            Image bgImage = bgPanel.AddModule<Image>();
            bgImage.Picture = new Texture("assets/textures/menu_bg.png");
            bgImage.AutoTile = true;
            bgImage.AutoTileScale = 1/2.0F;

            WObject logo = new WObject("Game Text Logo") { Parent = MenuWobject };
            Image logoImage = logo.AddModule<Image>();
            logoImage.Picture = new Texture("assets/textures/logo.png");
            logoImage.MinAnchor = new Vector2F(0.25F, 0.7F);
            logoImage.MaxAnchor = new Vector2F(0.75F, 0.85F);
            logoImage.KeepRatio = true;


            WObject btnPanel = new WObject("Buttons Panel") { Parent = MenuWobject };
            Image btnPanelImg = btnPanel.AddModule<Image>();
            btnPanelImg.Color = new Color256(1.0, 0.0, 1.0, 1.0);
            btnPanelImg.MinAnchor = new Vector2F(0.33F, 0.1F);
            btnPanelImg.MaxAnchor = new Vector2F(0.66F, 0.65F);
            //btnPanelImg.MinScale = Vector3F.One * 400.0F;

            Texture largeButtonTex = new Texture("assets/textures/button.png");

            WObject single = new WObject("Singleplayer Button") { Parent = btnPanel };
            Button btnSingle = single.AddModule<Button>();
            btnSingle.Background.Picture = largeButtonTex;
            btnSingle.MinAnchor = new Vector2F(0.0F, 0.0F);
            btnSingle.MaxAnchor = new Vector2F(1.0F, 1.0F);
            btnSingle.KeepRatio = true;
            btnSingle.HoverColor = new Color256(1.0,1.0,1.25,0.5);
            btnSingle.Label.Text = "           Singleplayer";
            btnSingle.Label.Color = new Color256(0.95, 0.95, 0.95, 1.0);
            btnSingle.Label.FontSize = 40.0F;
            btnSingle.OnHover += () => btnSingle.Label.Color = new Color256(1.0, 0.98, 0.6, 1.0);
            btnSingle.OnUnhover += () => btnSingle.Label.Color = new Color256(0.95, 0.95, 0.95, 1.0);

            /*
            WObject mult = new WObject("Multiplayer Button") { Parent = btnPanel };
            Button btnMult = mult.AddModule<Button>();
            btnMult.Background.Picture = largeButtonTex;
            btnMult.MinAnchor = new Vector2F(0.0F, 0.5F);
            btnMult.MaxAnchor = new Vector2F(1.0F, 1.0F);
            btnMult.KeepRatio = true;
            btnMult.HoverColor = new Color256(1.0, 1.0, 1.25, 1.0);
            btnMult.Label.Text = "           Multiplayer";
            btnMult.Label.Color = new Color256(0.5, 0.5, 0.5, 1.0);
            btnMult.Label.FontSize = 40.0F;
            //btnMult.OnHover += () => btnMult.Label.Color = new Color256(1.0, 0.98, 0.6, 1.0);
            //btnMult.OnUnhover += () => btnMult.Label.Color = new Color256(0.95, 0.95, 0.95, 1.0);

            btnMult.Locked = true;*/



            /*WObject mods = new WObject("Mods Button") { Parent = btnPanel };
            Button btnMods = mods.AddModule<Button>();
            btnMods.Background.Picture = largeButtonTex;
            btnMods.MinAnchor = new Vector2F(0.0F, 0.35F);
            btnMods.MaxAnchor = new Vector2F(1.0F, 0.425F);
            //btnMods.KeepRatio = true;
            btnMods.HoverColor = new Color256(1.0, 1.0, 1.25, 1.0);
            btnMods.Label.Text = "           Mods";
            btnMods.Label.Color = new Color256(0.5, 0.5, 0.5, 1.0);
            btnMods.Label.FontSize = 40.0F;
            //btnMult.OnHover += () => btnMult.Label.Color = new Color256(1.0, 0.98, 0.6, 1.0);
            //btnMult.OnUnhover += () => btnMult.Label.Color = new Color256(0.95, 0.95, 0.95, 1.0);

            btnMods.Locked = true;*/





           /* WObject options = new WObject("Mods Button") { Parent = btnPanel };
            Button btnOptions = options.AddModule<Button>();
            btnOptions.Background.Picture = largeButtonTex;
            btnOptions.MinAnchor = new Vector2F(0.0F, 0.2F);
            btnOptions.MaxAnchor = new Vector2F(1.0F, 0.275F);
            //btnOptions.KeepRatio = true;
            btnOptions.HoverColor = new Color256(1.0, 1.0, 1.25, 1.0);
            btnOptions.Label.Text = "           Options";
            btnOptions.Label.Color = new Color256(0.5, 0.5, 0.5, 1.0);
            btnOptions.Label.FontSize = 40.0F;
            //btnMult.OnHover += () => btnMult.Label.Color = new Color256(1.0, 0.98, 0.6, 1.0);
            //btnMult.OnUnhover += () => btnMult.Label.Color = new Color256(0.95, 0.95, 0.95, 1.0);

            btnOptions.Locked = true;*/

        }

        private static void Btn_OnClick()
        {
            throw new NotImplementedException();
        }

        public static void Show()
        {
            if (!MenuWobject) CreateMenu();


            MenuWobject.Enabled = true;
        }

        public static void Hide()
        {
            MenuWobject.Enabled = false;
        }
    }
}
