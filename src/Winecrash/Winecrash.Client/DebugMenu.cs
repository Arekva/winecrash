using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winecrash.Engine;
using Winecrash.Engine.GUI;

namespace Winecrash.Game
{
    public sealed class DebugMenu : Module
    {
        public static DebugMenu Instance { get; private set; }

        private Label lbFPS;
        private Label lbVersion;
        protected override void Creation()
        {
            if (Instance)
            {
                Delete();
                return;
            }

            Instance = this;

            this.WObject.Parent = Canvas.Main.WObject;
            lbFPS = this.WObject.AddModule<Label>();
            lbFPS.FontSize = 40.0F;
            lbFPS.MinAnchor = new Vector2F(0.0F, 0.0F);
            lbFPS.MaxAnchor = new Vector2F(1.0F, 1.0F);
            lbFPS.Left = 15;
            lbFPS.LineSpace = 1.0F;



            lbVersion = this.WObject.AddModule<Label>();
            lbVersion.FontSize = 40.0F;
            lbVersion.MinAnchor = new Vector2F(0.0F, 0.0F);
            lbVersion.MaxAnchor = new Vector2F(1.0F, 0.1F);
            lbVersion.Left = 15;

            lbVersion.Text = "Winecrash p0.2";
            lbVersion.Color = Color256.Orange;

        }

        int fpses;
        int frames;

        double timesincefpsupdate = 0.0F;
        double rate = 0.5F;

        string fpstext = "0 FPS";
        protected override void Update()
        {
            fpses += (int)(1D / Time.DeltaTime);
            timesincefpsupdate += Time.DeltaTime;
            frames++;


            if (timesincefpsupdate > rate)
            {
                timesincefpsupdate = 0.0D;

                int fps = (int)(fpses / frames);

                fpses = 0;
                frames = 0;

                fpstext = fps.ToString("D3") + " FPS";
            }

            string txt = fpstext;

            Vector3F pos = Player.Instance.WObject.Position;
            World.GlobalToLocal(pos, out Vector3I cpos, out Vector3I lpos);

            txt += "\n";
            txt += "XYZ: " + pos.X.ToString("0.000") + " / " + pos.Y.ToString("0.000") + " / " + pos.Z.ToString("0.000") + "\n";
            if (Player.Instance.ViewRayHit != null)
            {
                Vector3F n = Player.Instance.ViewRayHit.Value.Normal;

                Vector3F blockGlobalPosition = Player.Instance.ViewRayHit.Value.GlobalPosition;
                blockGlobalPosition.X -= blockGlobalPosition.X < 0.0F ? 1 : 0;
                blockGlobalPosition.Z -= blockGlobalPosition.Z < 0.0F ? 1 : 0;


                Vector3I blockGlobalUnitPosition = new Vector3I((int)blockGlobalPosition.X, (int)blockGlobalPosition.Y, (int)blockGlobalPosition.Z);//Player.Instance.ViewRayHit.Value.GlobalPosition;

                txt += "Pointed Block: " + Player.Instance.ViewRayHit.Value.Block.Identifier + "\n";
                txt += "Block Location XYZ: " + blockGlobalUnitPosition.X + " / " + blockGlobalUnitPosition.Y + " / " + blockGlobalUnitPosition.Z + "\n";
                txt += "Pointed normal: " + BlockFacesExtentions.Face(n).ToString() + $" ({n.X};{n.Y};{n.Z})";
            }
            else
            {
                txt += "Pointed Block: none";
            }

            lbFPS.Text = txt;

            if (Input.IsPressing(GameInput.Key("Debug")))
            {
                lbFPS.Enabled =! lbFPS.Enabled;
            }
        }
    }
}
