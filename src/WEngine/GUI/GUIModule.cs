using System;

namespace WEngine.GUI
{
    public abstract class GUIModule : Module
    {
        public Vector2D MinAnchor { get; set; } = Vector2D.Zero;
        public Vector2D MaxAnchor { get; set; } = Vector2D.One;

        public GUIModule ParentGUI { get; set; } = null;

        public Vector3D MinSize { get; set; } = Vector3D.One * Double.NegativeInfinity;
        public Vector3D MaxSize { get; set; } = Vector3D.One * Double.PositiveInfinity;

        public Vector2D MinScreenPercent { get; set; } = Vector2D.One * Double.NegativeInfinity;
        public Vector2D MaxScreenPercent { get; set; } = Vector2D.One * Double.PositiveInfinity;
        
        public virtual Vector3D GlobalPosition
        {
            get
            {
                /*if(this.ParentGUI == null)
                {
                    return this.WObject.Position;
                }*/

                double[] ganchors = GlobalScreenAnchors;

                Vector2D gMinAnchors = new Vector2D(ganchors[0], ganchors[1]);
                Vector2D gMaxAnchors = new Vector2D(ganchors[2], ganchors[3]);

                Vector2D half = (gMaxAnchors - gMinAnchors) / 2.0D;

                Vector2D screenSpacePosition = gMinAnchors + half;

                double horizontalShift = (GlobalRight / 4.0D) - (GlobalLeft / 4.0D);
                double verticalShift = (GlobalBottom / 4.0D) - (GlobalTop / 4.0D);

                Vector2D shift = new Vector2D(horizontalShift, verticalShift);

                return new Vector3D(Canvas.ScreenToUISpace(screenSpacePosition) + shift, Depth) + GlobalShift;
            }
        }

        public double GlobalRight
        {
            get
            {
                return this.ParentGUI == null ? this.Right : this.ParentGUI.GlobalRight + this.Right;
            }
        }

        public double GlobalLeft
        {
            get
            {
                return this.ParentGUI == null ? this.Left : this.ParentGUI.GlobalLeft + this.Left;
            }
        }

        public double GlobalTop
        {
            get
            {
                return this.ParentGUI == null ? this.Top : this.ParentGUI.Top + this.Top;
            }
        }

        public double GlobalBottom
        {
            get
            {
                return this.ParentGUI == null ? this.Bottom : this.ParentGUI.GlobalBottom + this.Bottom;
            }
        }
        
        public Vector3D Shift { get; set; }

        public Vector3D GlobalShift
        {
            get
            {
                return this.ParentGUI == null ? this.Shift : this.ParentGUI.GlobalShift + this.Shift;
            }
        }

        public virtual Vector3D GlobalScale
        {
            get//GlobalScreenAnchors
            {
                Vector3D totalExtentsScaled = new Vector3D(((Vector2D)Canvas.Main.Extents) * 2.0D, 1.0D) * this.WObject.Scale;


                double[] anchors = this.GlobalScreenAnchors;


                Vector2D minanchors = new Vector2D(anchors[0], anchors[1]);
                Vector2D maxanchors = new Vector2D(anchors[2], anchors[3]);

                Vector2D deltas = maxanchors - minanchors;

                totalExtentsScaled.XY *= deltas;

                double horizontalScale = -(GlobalRight / 2.0D) - (GlobalLeft / 2.0D);
                double verticalScale = -(GlobalBottom / 2.0D) - (GlobalTop / 2.0D);

                Vector3D sca = totalExtentsScaled * this.WObject.Scale + new Vector3D(horizontalScale, verticalScale, 1.0D);

                return sca;
            }
        }

        /// <summary>
        /// Get the global anchor position of the object, depending of the parents. Indices of the arary => xmin[0], ymin[1], xmax[2], ymax[3]
        /// </summary>
        public virtual double[] GlobalScreenAnchors
        {
            get
            {
                double[] anchors = new double[4];
                double xMin, yMin, xMax, yMax;

                if (this.ParentGUI == null)
                {
                    xMin = this.MinAnchor.X;
                    yMin = this.MinAnchor.Y;
                    xMax = this.MaxAnchor.X;
                    yMax = this.MaxAnchor.Y;
                }

                else
                {
                    double[] panchors = this.ParentGUI.GlobalScreenAnchors;

                    xMin = WMath.Remap(this.MinAnchor.X, 0.0D, 1.0D, panchors[0], panchors[2]);      
                    yMin = WMath.Remap(this.MinAnchor.Y, 0.0D, 1.0D, panchors[1], panchors[3]);                  
                    xMax = WMath.Remap(this.MaxAnchor.X, 0.0D, 1.0D, panchors[0], panchors[2]);               
                    yMax = WMath.Remap(this.MaxAnchor.Y, 0.0D, 1.0D, panchors[1], panchors[3]);
                }

                double xCentre = xMin + ((xMax - xMin) / 2.0D);
                double yCentre = yMin + ((yMax - yMin) / 2.0D);

                if (this is IRatioKeeper keepr && keepr.KeepRatio)
                {
                    double screenRatio = (double)Canvas.Main.Size.X / (double)Canvas.Main.Size.Y;
                    double invScreenRatio = 1F / screenRatio;

                    double xRatio = keepr.Ratio;
                    double yRatio = 1D / keepr.Ratio;

                    double xCurrentRatio = xMax / yMax;
                    double yCurrentRatio = 1D / xCurrentRatio;

                    //todo: ratio < 1.0
                    double x = xMax - xCentre;
                    double y = x * yRatio * screenRatio;

                    yMin = yCentre - y;
                    yMax = yCentre + y;
                }

                Vector3D halfExtents = new Vector3D(Canvas.Main.Extents, 0.5D);
                       
                Vector3D totalExtents = halfExtents * 2.0D;
                Vector3D sizes = new Vector3D(xMax - xMin, yMax - yMin, 0.0D);
                Vector3D scaledSizes = totalExtents * sizes;
                       
                Vector3D minSize = this.MinSize;
                Vector3D maxSize = this.MaxSize;

                scaledSizes.X = WMath.Clamp(scaledSizes.X, minSize.X, maxSize.X);
                scaledSizes.Y = WMath.Clamp(scaledSizes.Y, minSize.Y, maxSize.Y);
                scaledSizes.Z = WMath.Clamp(scaledSizes.Z, minSize.Z, maxSize.Z);

                sizes = scaledSizes / totalExtents;
                Vector3D halfSizes = sizes / 2.0D;

                xMin = xCentre - halfSizes.X;
                xMax = xCentre + halfSizes.X;
                yMin = yCentre - halfSizes.Y;
                yMax = yCentre + halfSizes.Y;

                anchors[0] = xMin;
                anchors[1] = yMin;
                anchors[2] = xMax;
                anchors[3] = yMax;

                return anchors;
            }
        }

        /// <summary>
        /// Distance from the top of the anchor
        /// </summary>
        public double Top { get; set; } = 0.0D;
        /// <summary>
        /// Distance from the bottom of the anchor
        /// </summary>
        public double Bottom { get; set; } = 0.0D;
        /// <summary>
        /// Distance from the right of the anchor
        /// </summary>
        public double Right { get; set; } = 0.0D;
        /// <summary>
        /// Distance from the Left of the anchor
        /// </summary>
        public double Left { get; set; } = 0.0D;
        /// <summary>
        /// Depth
        /// </summary>
        public double Depth { get; set; } = 0.0D;
    }
}
