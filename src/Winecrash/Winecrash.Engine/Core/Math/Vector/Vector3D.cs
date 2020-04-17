using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Vector3D : IVectorable, IComparable, IComparable<Vector3D>, IEquatable<Vector3D>, IFormattable
    {
        public int Dimensions { get; }

        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #region Multi Dimensional Accessors
        public Vector2D XY
        {
            get
            {
                return new Vector2D(this.X, this.Y);
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
                return new Vector2D(this.Y, this.X);
            }

            set
            {
                this.Y = value.X;
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

        #endregion

        #region Directions

        /// <summary>
        /// Positive X
        /// </summary>
        public static Vector3D Right
        {
            get
            {
                return new Vector3D(1.0D, 0.0D, 0.0D);
            }
        }
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector3D Left
        {
            get
            {
                return new Vector3D(-1.0D, 0.0D, 0.0D);
            }
        }

        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector3D Up
        {
            get
            {
                return new Vector3D(0.0D, 1.0D, 0.0D);
            }
        }
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector3D Down
        {
            get
            {
                return new Vector3D(0.0D, -1.0D, 0.0D);
            }
        }

        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3D Forward
        {
            get
            {
                return new Vector3D(0.0D, 0.0D, 1.0D);
            }
        }
        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3D Backward
        {
            get
            {
                return new Vector3D(0.0D, 0.0D, -1.0D);
            }
        }

        #endregion

        /// <summary>
        /// All zero
        /// </summary>
        public static Vector3D Zero
        {
            get
            {
                return new Vector3D(0.0D);
            }
        }

        /// <summary>
        /// All postive
        /// </summary>
        public static Vector3D One
        {
            get
            {
                return new Vector3D(1.0D);
            }
        }


        public double SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector3D Normalized
        {
            get
            {
                return NormalizeVector3D(this);
            }
        }
        #endregion

        #region Constructors
        public Vector3D(Vector2D xy, double z)
        {
            this.X = xy.X;
            this.Y = xy.Y;
            this.Z = z;

            this.Dimensions = 3;
        }
        public Vector3D(double x, Vector2D yz)
        {
            this.X = x;
            this.Y = yz.X;
            this.Z = yz.Y;

            this.Dimensions = 3;
        }
        public Vector3D(double values)
        {
            this.X = values;
            this.Y = values;
            this.Z = values;

            this.Dimensions = 3;
        }
        public Vector3D(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0.0D;

            this.Dimensions = 3;
        }
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            this.Dimensions = 3;
        }
        #endregion

        #region Methods

        public Vector3D Normalize()
        {
            return this = NormalizeVector3D(this);
        }

        private static Vector3D NormalizeVector3D(Vector3D vector)
        {
            if (vector == Vector3D.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3D d &&
                   X == d.X &&
                   Y == d.Y &&
                   Z == d.Z;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + this.X.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Z.GetHashCode();
            return hashCode;
        }

        #region ToString
        public override string ToString()
        {
            return $"Vector3D({this.X.ToString()};{this.Y.ToString()};{this.Z.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector3D({this.X.ToString(provider)};{this.Y.ToString(provider)};{this.Z.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector3D({this.X.ToString(format)};{this.ToString(format)};{this.Z.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector3D({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)};{this.Z.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector3D v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector3D value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector3D o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y &&
                this.Z == o.Z;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }
        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }
        public static Vector3D operator +(Vector3D v, double n)
        {
            return new Vector3D(v.X + n, v.Y + n, v.Z + n);
        }
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(v.X * -1.0D, v.Y * -1.0D, v.Z * -1.0D);
        }
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3D operator *(Vector3D v, double n)
        {
            return new Vector3D(v.X * n, v.Y * n, v.Z * n);
        }
        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static Vector3D operator /(Vector3D v, double n)
        {
            return new Vector3D(v.X / n, v.Y / n, v.Z / n);
        }
        public static Vector3D operator /(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }
        #endregion
    }
}
