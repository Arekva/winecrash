using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public class Model : GUIModule, IRatioKeeper
    {
        public ModelRenderer Renderer;

        public bool KeepRatio { get; set; } = false;
        public float Ratio { get; set; } = 1.0F;

        public float SizeX => 1.0F;
        public float SizeY => 1.0F;


        internal override Vector3F GlobalScale
        {
            get//GlobalScreenAnchors
            {
                Vector3F totalExtentsScaled = new Vector3F(((Vector2F)Canvas.Main.Extents) * 2.0F, 1.0F) * (this.WObject.Scale / 2.0F);


                float[] anchors = this.GlobalScreenAnchors;


                Vector2F minanchors = new Vector2F(anchors[0], anchors[1]);
                Vector2F maxanchors = new Vector2F(anchors[2], anchors[3]);

                Vector2F deltas = maxanchors - minanchors;

                totalExtentsScaled.XY *= deltas;

                float horizontalScale = -(GlobalRight / 2.0F) - (GlobalLeft / 2.0F);
                float verticalScale = -(GlobalBottom / 2.0F) - (GlobalTop / 2.0F);

                Vector3F sca = totalExtentsScaled * this.WObject.Scale + new Vector3F(horizontalScale, verticalScale, 1.0F);

                sca.X = WMath.Clamp(sca.X, MinScale.X, MaxScale.X);
                sca.Y = WMath.Clamp(sca.Y, MinScale.Y, MaxScale.Y);
                sca.Z = WMath.Clamp(sca.Z, MinScale.Z, MaxScale.Z);

                if (KeepRatio)
                {
                    float smallest = sca.X;

                    if (sca.Y < sca.X)
                    {
                        smallest = sca.Y;
                    }

                    sca = new Vector3F(smallest, smallest, smallest);
                }

                return sca;
            }
        }

        public float GlobalRatio
        {
            get
            {
                if (this.WObject.Parent != null && this.WObject.Parent is IRatioKeeper keepr && keepr.KeepRatio)
                {
                    return keepr.GlobalRatio * Ratio;
                }

                else
                {
                    return Ratio;
                }
            }
        }

        public Vector3F MaxScale { get; set; } = Vector3F.One * float.MaxValue;
        public Vector3F MinScale { get; set; } = -Vector3F.One * float.MaxValue;

        protected internal override void Creation()
        {
            this.WObject.Layer = 1L << 48;

            Renderer = this.WObject.AddModule<ModelRenderer>();
            Renderer.Model = this;

            GUIModule guimod = this.WObject.Parent?.GetModule<GUIModule>();
            if (guimod != null)
            {
                this.ParentGUI = guimod;
            }
        }

        protected internal override void OnDelete()
        {
            Renderer.Delete();
        }

        protected internal override void OnEnable()
        {
            Renderer.Enabled = true;
        }
        protected internal override void OnDisable()
        {
            Renderer.Enabled = false;
        }
    }
}
