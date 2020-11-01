namespace WEngine.GUI
{
    public class Model : GUIModule, IRatioKeeper
    {
        public ModelRenderer Renderer;

        public bool KeepRatio { get; set; } = false;
        public double Ratio { get; set; } = 1.0D;

        public double SizeX => 1.0D;
        public double SizeY => 1.0D;


        public override Vector3D GlobalScale
        {
            get//GlobalScreenAnchors
            {
                Vector3D totalExtentsScaled = new Vector3D((Vector2D)Canvas.Main.Extents * 2.0D, 1.0D) * (this.WObject.Scale / 2.0D);
                
                double[] anchors = this.GlobalScreenAnchors;


                Vector2D minanchors = new Vector2D(anchors[0], anchors[1]);
                Vector2D maxanchors = new Vector2D(anchors[2], anchors[3]);

                Vector2D deltas = maxanchors - minanchors;

                totalExtentsScaled.XY *= deltas;

                double horizontalScale = -(GlobalRight / 2.0D) - (GlobalLeft / 2.0D);
                double verticalScale = -(GlobalBottom / 2.0D) - (GlobalTop / 2.0D);

                Vector3D sca = totalExtentsScaled * this.WObject.Scale + new Vector3D(horizontalScale, verticalScale, 1.0D);

                sca.X = WMath.Clamp(sca.X, MinScale.X, MaxScale.X);
                sca.Y = WMath.Clamp(sca.Y, MinScale.Y, MaxScale.Y);
                sca.Z = WMath.Clamp(sca.Z, MinScale.Z, MaxScale.Z);

                if (KeepRatio)
                {
                    double smallest = sca.X;

                    if (sca.Y < sca.X)
                    {
                        
                        smallest = sca.Y;
                    }

                    sca = new Vector3D(smallest, smallest, smallest);
                }

                return sca;
            }
        }

        public double GlobalRatio
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
