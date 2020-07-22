using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public abstract class GUIModule : Module
    {
        public Vector2F MinAnchor { get; set; } = Vector2F.Zero;
        public Vector2F MaxAnchor { get; set; } = Vector2F.One;

        public GUIModule ParentGUI { get; set; } = null;


        internal virtual Vector3F GlobalPosition
        {
            get
            {
                if(this.ParentGUI == null)
                {
                    return this.WObject.Position;
                }

                float[] ganchors = GlobalScreenAnchors;

                Vector2F gMinAnchors = new Vector2F(ganchors[0], ganchors[1]);
                Vector2F gMaxAnchors = new Vector2F(ganchors[2], ganchors[3]);

                Vector2F half = (gMaxAnchors - gMinAnchors) / 2.0F;

                Vector2F screenSpacePosition = gMinAnchors + half;

                float horizontalShift = (GlobalRight / 4.0F) - (GlobalLeft / 4.0F);
                float verticalShift = (GlobalBottom / 4.0F) - (GlobalTop / 4.0F);

                Vector2F shift = new Vector2F(horizontalShift, verticalShift);


                //Vector3F sca = new Vector3F(Canvas.ScreenToUISpace(screenSpacePosition) + shift;

                //sca.X = WMath.Clamp(sca.X, Image.MinScale.X, Image.MaxScale.X);
                //sca.Y = WMath.Clamp(sca.Y, Image.MinScale.Y, Image.MaxScale.Y);
                //sca.Z = WMath.Clamp(sca.Z, Image.MinScale.Z, Image.MaxScale.Z);

                return new Vector3F(Canvas.ScreenToUISpace(screenSpacePosition) + shift, Depth);
            }
        }

        internal float GlobalRight
        {
            get
            {
                return this.ParentGUI == null ? this.Right : this.ParentGUI.GlobalRight + this.Right;
            }
        }

        internal float GlobalLeft
        {
            get
            {
                return this.ParentGUI == null ? this.Left : this.ParentGUI.GlobalLeft + this.Left;
            }
        }

        internal float GlobalTop
        {
            get
            {
                return this.ParentGUI == null ? this.Top : this.ParentGUI.Top + this.Top;
            }
        }

        internal float GlobalBottom
        {
            get
            {
                return this.ParentGUI == null ? this.Bottom : this.ParentGUI.GlobalBottom + this.Bottom;
            }
        }

        internal virtual Vector3F GlobalScale
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

                return totalExtentsScaled * this.WObject.Scale + new Vector3F(horizontalScale, verticalScale, 1.0F);
            }
        }

        /// <summary>
        /// Get the global anchor position of the object, depending of the parents. Indices of the arary => xmin[0], ymin[1], xmax[2], ymax[3]
        /// </summary>
        internal virtual float[] GlobalScreenAnchors
        {
            get
            {
                float[] anchors = new float[4];

                if(this.ParentGUI == null)
                {
                    anchors[0] = anchors[1] = 0.0F;
                    anchors[2] = anchors[3] = 1.0F;
                }

                else
                {
                    float[] panchors = this.ParentGUI.GlobalScreenAnchors;

                    Vector2F pmin = new Vector2F(panchors[0], panchors[1]);
                    Vector2F pmax = new Vector2F(panchors[2], panchors[3]);

                    if(this.ParentGUI is IRatioKeeper)
                    {


                        anchors[0] = WMath.Remap(this.MinAnchor.X, 0, 1, pmin.X, pmax.X); //x min
                        anchors[1] = WMath.Remap(this.MinAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min
                        anchors[2] = WMath.Remap(this.MaxAnchor.X, 0, 1, pmin.X, pmax.X); //x max
                        anchors[3] = WMath.Remap(this.MaxAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min
                    }

                    else
                    {
                        anchors[0] = WMath.Remap(this.MinAnchor.X, 0, 1, pmin.X, pmax.X); //x min
                        anchors[1] = WMath.Remap(this.MinAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min
                        anchors[2] = WMath.Remap(this.MaxAnchor.X, 0, 1, pmin.X, pmax.X); //x max
                        anchors[3] = WMath.Remap(this.MaxAnchor.Y, 0, 1, pmin.Y, pmax.Y); //y min
                    }
                }

                return anchors;
            }
        }

        /// <summary>
        /// Distance from the top of the anchor
        /// </summary>
        public float Top { get; set; } = 0.0F;
        /// <summary>
        /// Distance from the bottom of the anchor
        /// </summary>
        public float Bottom { get; set; } = 0.0F;
        /// <summary>
        /// Distance from the right of the anchor
        /// </summary>
        public float Right { get; set; } = 0.0F;
        /// <summary>
        /// Distance from the Left of the anchor
        /// </summary>
        public float Left { get; set; } = 0.0F;
        /// <summary>
        /// Depth
        /// </summary>
        public float Depth { get; set; } = 0.0F;
    }
}
