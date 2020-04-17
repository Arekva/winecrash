using System;

namespace Winecrash.Engine
{
    public struct Vector4D : IVectorable, IComparable, IComparable<Vector4D>, IEquatable<Vector4D>, IFormattable
    {
        public int Dimensions { get; }

        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        #region Multi dimensional accessors
        public Vector2D XY
        {
            get
            {
                return new Vector2D(X, Y);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        public Vector2D YX
        {
            get
            {
                return new Vector2D(Y, X);
            }

            set
            {
                this.Y = value.X;
                this.X = value.Y;
            }
        }
        public Vector2D XZ
        {
            get
            {
                return new Vector2D(this.X, this.Z);
            }

            set
            {
                this.X = value.X;
                this.Z = value.Y;
            }
        }
        public Vector2D ZX
        {
            get
            {
                return new Vector2D(this.Z, this.X);
            }

            set
            {
                this.Z = value.X;
                this.X = value.Y;
            }
        }
        public Vector2D XW
        {
            get
            {
                return new Vector2D(this.X, this.W);
            }

            set
            {
                this.X = value.X;
                this.W = value.Y;
            }
        }
        public Vector2D WX
        {
            get
            {
                return new Vector2D(this.W, this.X);
            }
            set
            {
                this.W = value.X;
                this.X = value.Y;
            }
        }
        public Vector2D YZ
        {
            get
            {
                return new Vector2D(this.Y, this.Z);
            }

            set
            {
                this.Y = value.X;
                this.Z = value.Y;
            }
        }
        public Vector2D ZY
        {
            get
            {
                return new Vector2D(this.Z, this.Y);
            }

            set
            {
                this.Z = value.X;
                this.Y = value.Y;
            }
        }
        public Vector2D YW
        {
            get
            {
                return new Vector2D(this.Y, this.W);
            }

            set
            {
                this.Y = value.X;
                this.W = value.Y;
            }
        }
        public Vector2D WY
        {
            get
            {
                return new Vector2D(this.W, this.Y);
            }

            set
            {
                this.W = value.X;
                this.Y = value.Y;
            }
        }
        public Vector2D ZW
        {
            get
            {
                return new Vector2D(this.Z, this.W);
            }

            set
            {
                this.Z = value.X;
                this.W = value.Y;
            }
        }
        public Vector2D WZ
        {
            get
            {
                return new Vector2D(this.W, this.Z);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
            }
        }
        public Vector3D XYZ
        {
            get
            {
                return new Vector3D(this.X, this.Y, this.Z);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }
        public Vector3D ZYX
        {
            get
            {
                return new Vector3D(this.Z, this.Y, this.X);
            }

            set
            {
                this.Z = value.X;
                this.Y = value.Y;
                this.X = value.Z;
            }
        }
        public Vector3D YZW
        {
            get
            {
                return new Vector3D(this.Y, this.Z, this.W);
            }

            set
            {
                this.Y = value.X;
                this.Z = value.Y;
                this.W = value.Z;
            }
        }
        public Vector3D WZY
        {
            get
            {
                return new Vector3D(this.W, this.Z, this.Y);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
                this.Y = value.Z;
            }
        }
        public Vector3D XZW
        {
            get
            {
                return new Vector3D(this.X, this.Z, this.W);
            }

            set
            {
                this.X = value.X;
                this.Z = value.Y;
                this.W = value.Z;
            }
        }
        public Vector3D WZX
        {
            get
            {
                return new Vector3D(this.W, this.Z, this.X);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
                this.X = value.Z;
            }
        }
        public Vector3D XYW
        {
            get
            {
                return new Vector3D(this.X, this.Y, this.W);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.W = value.Z;
            }
        }
        public Vector3D WYZ
        {
            get
            {
                return new Vector3D(this.W, this.Y, this.Z);
            }

            set
            {
                this.W = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }
        public Vector4D XYZW
        {
            get
            {
                return this;
            }

            set
            {
                this = value;
            }
        }
        public Vector4D WZYX
        {
            get
            {
                return new Vector4D(this.W, this.Z, this.Y, this.X);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
                this.Y = value.Z;
                this.X = value.W;
            }
        }
        #endregion

        #region Directions
        /// <summary>
        /// Positive X
        /// </summary>
        public static Vector4D Right
        {
            get
            {
                return new Vector4D(1.0D, 0.0D, 0.0D, 0.0D);
            }
        }
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector4D Left
        {
            get
            {
                return new Vector4D(-1.0D, 0.0D, 0.0D, 0.0D);
            }
        }

        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector4D Up
        {
            get
            {
                return new Vector4D(0.0D, 1.0D, 0.0D,0.0D);
            }
        }
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector4D Down
        {
            get
            {
                return new Vector4D(0.0D, -1.0D, 0.0D, 0.0D);
            }
        }

        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector4D Forward
        {
            get
            {
                return new Vector4D(0.0D, 0.0D, 1.0D, 0.0D);
            }
        }
        /// <summary>
        /// Negative Z
        /// </summary>
        public static Vector4D Backward
        {
            get
            {
                return new Vector4D(0.0D, 0.0D, -1.0D, 0.0D);
            }
        }

        /// <summary>
        /// Positive W
        /// </summary>
        public static Vector4D Ana
        {
            get
            {
                return new Vector4D(0.0D, 0.0D, 0.0D, 1.0D);
            }
        }
        /// <summary>
        /// Negative W
        /// </summary>
        public static Vector4D Kata
        {
            get
            {
                return new Vector4D(0.0D, 0.0D, 0.0D, -1.0D);
            }
        }
        #endregion

        /// <summary>
        /// All zero
        /// </summary>
        public static Vector4D Zero
        {
            get
            {
                return new Vector4D(0.0D);
            }
        }
        /// <summary>
        /// All positive
        /// </summary>
        public static Vector4D One
        {
            get
            {
                return new Vector4D(1.0D);
            }
        }

        public double SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector4D Normalized
        {
            get
            {
                return NormalizeVector4D(this);
            }
        }
        #endregion

        #region Constructors
        public Vector4D(Vector2D xy, double z, double w)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;
            this.W = w;

            this.Dimensions = 4;
        }
        public Vector4D(double x, Vector2D yz, double w)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;
            this.W = w;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y, Vector2D zw)
        {
            this.X = x;
            this.Y = y;
            this.Z = zw.X;
            this.W = zw.Y;

            this.Dimensions = 4;
        }
        public Vector4D(Vector2D xy, Vector2D zw)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = zw.X;
            this.W = zw.Y;

            this.Dimensions = 4;
        }
        public Vector4D(Vector3D xyz, double w)
        {
            this.X = xyz.X;
            this.Y = xyz.Y;
            this.Z = xyz.Z;
            this.W = w;

            this.Dimensions = 4;
        }
        public Vector4D(double x, Vector3D yzw)
        {
            this.X = x;
            this.Y = yzw.X;
            this.Z = yzw.Y;
            this.W = yzw.Z;

            this.Dimensions = 4;
        }
        public Vector4D(double values)
        {
            this.X = values;
            this.Y = values;
            this.Z = values;
            this.W = values;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0.0D;
            this.W = 0.0D;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 0.0D;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;

            this.Dimensions = 4;
        }
        #endregion

        #region Methods

        public Vector4D Normalize()
        {
            return this = NormalizeVector4D(this);
        }

        private static Vector4D NormalizeVector4D(Vector4D vector)
        {
            if (vector == Vector4D.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4D d &&
                   X == d.X &&
                   Y == d.Y &&
                   Z == d.Z &&
                   W == d.W;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + this.X.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Z.GetHashCode();
            hashCode = hashCode * -1521134295 + this.W.GetHashCode();
            return hashCode;
        }

        #region ToString
        public override string ToString()
        {
            return $"Vector4D({this.X.ToString()};{this.Y.ToString()};{this.Z.ToString()};{this.W.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector4D({this.X.ToString(provider)};{this.Y.ToString(provider)};{this.Z.ToString(provider)};{this.W.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector4D({this.X.ToString(format)};{this.ToString(format)};{this.Z.ToString(format)};{this.W.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector4D({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)};{this.Z.ToString(format, provider)};{this.W.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector4D v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector4D value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector4D o)
        {
            return 
                this.X == o.X && 
                this.Y == o.Y && 
                this.Z == o.Z &&
                this.W == o.W;
        }

        #endregion

        #region Operators
        public static bool operator == (Vector4D v1, Vector4D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;
        }
        public static bool operator != (Vector4D v1, Vector4D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z || v1.W != v2.W;
        }
        public static Vector4D operator + (Vector4D v, double n)
        {
            return new Vector4D(v.X + n, v.Y + n, v.Z + n, v.W + n);
        }
        public static Vector4D operator + (Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }
        public static Vector4D operator - (Vector4D v)
        {
            return new Vector4D(v.X * -1.0D, v.Y * -1.0D, v.Z * -1.0D, v.W * -1.0D);
        }
        public static Vector4D operator - (Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }
        public static Vector4D operator * (Vector4D v, double n)
        {
            return new Vector4D(v.X * n, v.Y * n, v.Z * n, v.W * n);
        }
        public static Vector4D operator * (Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }
        public static Vector4D operator /(Vector4D v, double n)
        {
            return new Vector4D(v.X / n, v.Y / n, v.Z / n, v.W / n);
        }
        public static Vector4D operator /(Vector4D v1, Vector4D v2)
        {
            return new Vector4D(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
        }
        #endregion
    }
}


