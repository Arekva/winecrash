using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game
{
    public sealed class EscapeMenu : Module
    {
        public static EscapeMenu Instance { get; private set; }

        private Label lbMenu;
        private Image imgBack;
        
        protected override void Creation()
        {
            if (Instance)
            {
                Delete();
                return;
            }

            Instance = this;

            this.WObject.Parent = Canvas.Main.WObject;
            lbMenu = this.WObject.AddModule<Label>();
            lbMenu.FontSize = 60.0F;
            lbMenu.MinAnchor = new Vector2F(0.4F, 0.30F);
            lbMenu.MaxAnchor = new Vector2F(0.7F, 0.51F);
            lbMenu.Enabled = false;

            imgBack = this.WObject.AddModule<Image>();
            imgBack.Color = new Color256(0, 0, 0, 0.3);
            imgBack.Enabled = false;
        }

        string menutxt = "Game Paused";

        protected override void Update()
        {
            lbMenu.Text = menutxt;

            if (Input.IsPressing(GameInput.Key("Paused")))
            {
                //WObject.Find("Crosshair").Enabled = !WObject.Find("Crosshair").Enabled;
                //imgBack.Enabled = !imgBack.Enabled;
                lbMenu.Enabled = !lbMenu.Enabled;
                Input.LockMode = Input.LockMode == CursorLockModes.Free ? CursorLockModes.Lock : CursorLockModes.Free;
                if (lbMenu.Enabled == true)
                {
                    Time.TimeScale = 0;
                    Time.FixedTimeScale = 0;

                    Input.CursorVisible = true;
                }
                else
                {
                    Time.TimeScale = 1;
                    Time.FixedTimeScale = 1;

                    Input.CursorVisible = false;
                }
            }
        }
    }
}
