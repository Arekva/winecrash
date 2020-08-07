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

        public float SizeX => (float)_Picture.Size.X;
        public float SizeY => (float)_Picture.Size.Y;

        public float Ratio
        {
            get
            {
                return (float)_Picture.Size.X / (float)_Picture.Size.Y;
            }
        }
        public float GlobalRatio
        {
            get
            {
                if(this.WObject.Parent != null && this.WObject.Parent is IRatioKeeper keepr)
                {
                    return keepr.GlobalRatio * Ratio;
                }

                else
                {
                    return Ratio;
                }
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

        private Vector2F _Tiling = Vector2F.One;
        public Vector2F Tiling
        {
            get
            {
                return _Tiling;
            }

            set
            {
                _Tiling = value;

                if(AutoTile)
                {
                    Vector2F scale = this._Picture.Size;

                    float ratio = scale.X / scale.Y;

                    scale.Y = scale.X;
                    scale.X *= ratio;

                    Vector2F canSca = Canvas.Main.Size.XY;
                    float canRatio = canSca.X / canSca.Y;

                    Vector2F canPct = scale / canSca;
                    scale *= AutoTileScale;


                    //scale *= canPct;


                    //scale.X *= canRatioX;
                    //scale.Y *= 1/ canPct;

                    scale.X *= (canSca.X / canSca.Y);

                    this.Renderer.Material.SetData("tiling", (OpenTK.Vector2)scale);
                }

                else
                {
                    this.Renderer.Material.SetData("tiling", (OpenTK.Vector2)value);
                }
            }
        }



        public bool AutoTile { get; set; } = false;
        public float AutoTileScale { get; set; } = 1.0F;

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

            Tiling = _Tiling;
            Viewport.Instance.Resize += Instance_Resize;
            Viewport.Instance.WindowStateChanged += Instance_Resize;
            Viewport.Instance.WindowBorderChanged += Instance_Resize;

            GUIModule guimod = this.WObject.Parent?.GetModule<GUIModule>();
            if(guimod != null)
            {
                this.ParentGUI = guimod;
            }
        }

        private void Instance_Resize(object sender, EventArgs e)
        {
            Tiling = _Tiling;
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
