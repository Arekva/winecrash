using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public sealed class Image : GUIModule, IRatioKeeper
    {
        private ImageRenderer Renderer;

        private Texture _Picture = Texture.Blank;

        public bool KeepRatio { get; set; } = false;

        public float Ratio
        {
            get
            {
                return (float)_Picture.Size.X / (float)_Picture.Size.Y;
            }
        }

        internal override Vector3F GlobalScale
        {
            get//GlobalScreenAnchors
            {
                Vector3F totalExtentsScaled = new Vector3F(((Vector2F)Canvas.Main.Extents) * 2.0F, 1.0F) * this.WObject.Scale;


                float[] anchors = this.GlobalScreenAnchors;


                Vector2F minanchors = new Vector2F(anchors[0], anchors[1]);
                Vector2F maxanchors = new Vector2F(anchors[2], anchors[3]);

                Vector2F deltas = maxanchors - minanchors;

                totalExtentsScaled.XY *= deltas;

                float horizontalScale = -(GlobalRight / 2.0F) - (GlobalLeft / 2.0F);
                float verticalScale = -(GlobalBottom / 2.0F) - (GlobalTop / 2.0F);

                Vector3F sca = totalExtentsScaled * this.WObject.Scale + new Vector3F(horizontalScale, verticalScale, 1.0F);

                /*sca.X = WMath.Clamp(sca.X, MinScale.X, MaxScale.X);
                sca.Y = WMath.Clamp(sca.Y, MinScale.Y, MaxScale.Y);
                sca.Z = WMath.Clamp(sca.Z, MinScale.Z, MaxScale.Z);*/

                if (KeepRatio)
                {
                    float smallest = sca.X;

                    if (sca.Y < sca.X)
                    {
                        smallest = sca.Y;
                    }

                    if (smallest == sca.X)
                    {
                        sca = new Vector3F(sca.Y * Ratio, sca.Y, sca.Z);
                    }
                    else
                    {
                        sca = new Vector3F(sca.X, sca.X / Ratio, sca.Z);
                    }
                }

                return sca;
            }
        }

        internal override float[] GlobalScreenAnchors
        {
            get
            {
                float[] anchors = new float[4];

                if (this.ParentGUI == null)
                {
                    anchors[0] = anchors[1] = 0.0F;
                    anchors[2] = anchors[3] = 1.0F;
                }

                else
                {
                    float[] panchors = this.ParentGUI.GlobalScreenAnchors;

                    Vector2F pmin = new Vector2F(panchors[0], panchors[1]);
                    Vector2F pmax = new Vector2F(panchors[2], panchors[3]);

                    anchors[0] = WMath.Remap(this.MinAnchor.X, 0, 1, pmin.X, pmax.X); //x min
                    anchors[1] = WMath.Remap(this.MinAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min
                    anchors[2] = WMath.Remap(this.MaxAnchor.X, 0, 1, pmin.X, pmax.X); //x max
                    anchors[3] = WMath.Remap(this.MaxAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min            
                }

                return anchors;
            }
        }

        public Texture Picture
        {
            get
            {
                return this._Picture;
            }

            set
            {
                this._Picture = value;

                this.Renderer.Material.SetData<Texture>("albedo", value);
            }
        }

        private Color256 _Color = new Color256(1.0, 1.0, 1.0, 1.0);
        public Color256 Color
        {
            get
            {
                return this._Color;
            }

            set
            {
                this._Color = value;

                this.Renderer.Material.SetData<OpenTK.Vector4>("color", value);
            }
        }

        public Vector3F MaxScale { get; set; } = Vector3F.One * float.MaxValue;
        public Vector3F MinScale { get; set; } = -Vector3F.One * float.MaxValue;

        protected internal override void Creation()
        {
            this.WObject.Layer = 1L << 48;

            Renderer = this.WObject.AddModule<ImageRenderer>();
            
            Renderer.Material = new Material(Shader.Find("Unlit"));
            Renderer.Image = this;

            this.Renderer.Material.SetData<OpenTK.Vector4>("color", _Color);
            this.Renderer.Material.SetData<Texture>("albedo", _Picture);

            Renderer.Material.SetData<OpenTK.Vector2>("tiling", new Vector2D(1.0,1.0));

            GUIModule guimod = this.WObject.Parent?.GetModule<GUIModule>();
            if(guimod != null)
            {
                this.ParentGUI = guimod;
            }
        }

        protected internal override void OnDelete()
        {
            Renderer.Delete();
            _Picture = null;
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
