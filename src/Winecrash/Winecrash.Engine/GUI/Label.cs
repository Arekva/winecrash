using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public sealed class Label : GUIModule
    {
        public string Text { get; set; } = null;

        public float WordSpace = 0.33F; // defaults to 1.0 pixel, for each chars being 1.0 pixel
        public float LineSpace = 1.0F;

        public LabelRenderer Renderer { get; set; } = null;
        public Font FontFamilly { get; set; } = Font.Find("Pixelized");

        public bool Fill { get; set; } = false;

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
                
            }
        }

        public float FontSize { get; set; } = 16F;

        protected internal override void Creation()
        {
            this.WObject.Layer = 1L << 48;

            Renderer = this.WObject.AddModule<LabelRenderer>();
            Renderer.Material = new Material(Shader.Find("Text"));
            Renderer.Label = this;

            this.ParentGUI = this.WObject.Parent.GetModule<GUIModule>();

            
            Renderer.Material.SetData<OpenTK.Vector4>("color", Color);
        }

        protected internal override void LateUpdate()
        {
            //Debug.Log(FontFamilly.Glyphs.Map);
            if(FontFamilly.Glyphs.Map != null && Renderer.Material.GetData<Texture>("albedo") == Texture.Blank)
            {
                //Debug.Log("Loaded font texture into Label's material");
                Renderer.Material.SetData<Texture>("albedo", FontFamilly.Glyphs.Map);
            }
                
        }

        protected internal override void OnDelete()
        {
            Renderer.Label = null;
            Renderer = null;

            FontFamilly = null;
        }
    }
}
