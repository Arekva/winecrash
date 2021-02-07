using System;

namespace WEngine.GUI
{
    public sealed class Image : GUIModule, IRatioKeeper
    {
        public ImageRenderer Renderer { get; private set; }

        private Texture _Picture = Texture.Blank;

        public bool KeepRatio { get; set; } = false;

        public double SizeX => (float)_Picture.Size.X;
        public double SizeY => (float)_Picture.Size.Y;

        public double Ratio
        {
            get
            {
                return (double)_Picture.Size.X / (double)_Picture.Size.Y;
            }
        }
        public double GlobalRatio
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

        public Material Material
        {
            get
            {
                return Renderer.Material;
            }
            set
            {
                Renderer.Material = value;
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

        protected internal override void Creation()
        {
            this.WObject.Layer = 1L << 48;

            Renderer = this.WObject.AddModule<ImageRenderer>();
            
            Renderer.Material = new Material(Shader.Find("Unlit"));
            Renderer.Image = this;

            SetupMaterial();
            
            Graphics.Window.OnResizing += Instance_Resize;
            //Viewport.Instance.WindowStateChanged += Instance_Resize;
            //Viewport.Instance.WindowBorderChanged += Instance_Resize;

            GUIModule guimod = this.WObject.Parent?.GetModule<GUIModule>();
            if(guimod != null)
            {
                this.ParentGUI = guimod;
            }
        }
        
        public void SetupMaterial()
        {
            this.Renderer.Material.SetData<OpenTK.Vector4>("color", _Color);
            this.Renderer.Material.SetData<Texture>("albedo", _Picture);

            Tiling = _Tiling;
        }

        private void Instance_Resize(object sender, EventArgs e)
        {
            Graphics.Window.InvokeUpdate(() =>
            this.Tiling = _Tiling);
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
