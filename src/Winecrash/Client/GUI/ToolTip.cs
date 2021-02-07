using WEngine;
using WEngine.GUI;

namespace Winecrash.GUI
{
    public class ToolTip : GUIModule
    {
        public Image BackgroundImage { get; set; }
        public Label Label { get; set; }
        
        public Vector2D Offset { get; set; }

        protected override void Creation()
        {
            base.Creation();


            WObject bgImageW = new WObject("Background Image") {Parent = this.WObject, Layer = (ulong)Layers.UI};
            
            BackgroundImage = bgImageW.AddModule<Image>();
            BackgroundImage.Color = new Color256(0.0,0.0,0.0,0.95);

            WObject labelW = new WObject("Label") {Parent = bgImageW, Layer = (ulong)Layers.UI};
            Label = labelW.AddModule<Label>();
            Label.Text = "Nullref";
            Label.AutoSize = true;
            Label.Aligns = TextAligns.Middle;
            Offset = new Vector2D(-10, 10);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            
            Vector2D rawmp = Input.MousePosition;
            Vector2D ss = Graphics.Window.SurfaceResolution;
            Vector2D shift = new Vector2D(
                WMath.Remap(rawmp.X, -(ss.X/2), ss.X/2, 0, ss.X) - ss.X,
                WMath.Remap(rawmp.Y, -(ss.Y/2), ss.Y/2, 0, ss.Y)
            );

            this.Shift = new Vector3D(shift + Offset, -4000);

            double xmax = (Label.CurrentWidth / ss.X) * 4;
            double padding = 0.025;
            this.MaxAnchor = new Vector2D(xmax+padding, MaxAnchor.Y);
        }

        protected override void OnDelete()
        {
            base.OnDelete();

            BackgroundImage.WObject.Delete();
            BackgroundImage = null;
            Label = null;
        }
    }
}