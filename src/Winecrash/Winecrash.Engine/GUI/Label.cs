namespace WEngine.GUI
{
    public class Label : GUIModule
    {
        private string _Text = null;
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                _Text = value;

                if (value == null)
                {
                    Lines = null;
                    LinesMaxWidth = 0;
                }
                else
                {
                    string[] lines = value.Split('\n');

                    Lines = lines;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        LinesMaxWidth = WMath.Max(LinesMaxWidth, lines[i].Length);
                    }
                }
            }
        }
        internal string[] Lines { get; private set; } = null;
        public TextAligns Aligns { get; set; } = TextAligns.Up | TextAligns.Left;
        public int LinesMaxWidth { get; private set; } = 0;

        public bool Shadows { get; set; } = true;
        public double ShadowDistance { get; set; } = 0.1D;

        public double WordSpace { get; set; } = 0.33D; // defaults to 1.0 pixel, for each chars being 1.0 pixel
        public double LineSpace { get; set; } = 1.00D;

        public LabelRenderer Renderer { get; set; } = null;
        public Font FontFamilly { get; set; } = Font.Find("Pixelized");

        public bool Fill { get; set; } = false;

        public Color256 Color { get; set; } = Color256.White;
        public Color256 ShadowColor { get; set; } = new Color256(1 / 4.0D, 1/4.0D, 1/4.0D, 0.75F);

        public double FontSize { get; set; } = 16.0D;

        public bool AutoSize { get; set; } = false;

        public double Rotation { get; set; } = 0.0D;

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
