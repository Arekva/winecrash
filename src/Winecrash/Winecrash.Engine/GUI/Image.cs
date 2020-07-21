using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public sealed class Image : GUIModule
    {
        private ImageRenderer Renderer;

        private Texture _Picture = Texture.Blank;

        public bool KeepRatio { get; set; } = false;

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

        protected internal override void Creation()
        {
            this.WObject.Layer = 1L << 48;

            Renderer = this.WObject.AddModule<ImageRenderer>();
            
            Renderer.Material = new Material(Shader.Find("Unlit"));
            Renderer.Image = this;

            this.Renderer.Material.SetData<OpenTK.Vector4>("color", _Color);
            this.Renderer.Material.SetData<Texture>("albedo", _Picture);

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
