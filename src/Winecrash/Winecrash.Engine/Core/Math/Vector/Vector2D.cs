using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Vector2D : IVectorable, IComparable, IComparable<Vector2D>, IEquatable<Vector2D>, IFormattable
    {
        public int Dimensions { get; }


        #region Properties
        public double X { get; set; }
        public double Y { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2D XY
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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

        public static Vector2D Right
        {
            get
            {
                return new Vector2D(1.0D, 0.0D);
            }
        }
        public static Vector2D Left
        {
            get
            {
                return new Vector2D(-1.0D, 0.0D);
            }
        }
        public static Vector2D Up
        {
            get
            {
                return new Vector2D(0.0D, 1.0D);
            }
        }
        public static Vector2D Down
        {
            get
            {
                return new Vector2D(0.0D, -1.0D);
            }
        }

        public static Vector2D Zero
        {
            get
            {
                return new Vector2D(0.0D);
            }
        }
        public static Vector2D One
        {
            get
            {
                return new Vector2D(1.0D);
            }
        }

        public double SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector2D Normalized
        {
            get
            {
                return NormalizeVector2D(this);
            }
        }
        #endregion

        #region Constructors
        public Vector2D(double values)
        {
            this.X = values;
            this.Y = values;

            this.Dimensions = 2;
        }
        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;

            this.Dimensions = 2;
        }
        #endregion

        #region Methods
        public static double Distance(Vector2D v1, Vector2D v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static Vector2D Dot(Vector2D v1, Vector2D v2)
        {
            return v1 * v2;
        }
        public static double Angle(Vector2D v1, Vector2D v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length);
        }

        public Vector2D Normalize()
        {
            return this = NormalizeVector2D(this);
        }

        private static Vector2D NormalizeVector2D(Vector2D vector)
        {
            if (vector == Vector2D.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2D d &&
                   X == d.X &&
                   Y == d.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + this.X.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            return hashCode;
        }

        #region ToString
        public override string ToString()
        {
            return $"Vector2D({this.X.ToString()};{this.Y.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector2D({this.X.ToString(provider)};{this.Y.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector2D({this.X.ToString(format)};{this.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector2D({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector2D v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector2D value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector2D o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }
        public static Vector2D operator +(Vector2D v, double n)
        {
            return new Vector2D(v.X + n, v.Y + n);
        }
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(v.X * -1.0D, v.Y * -1.0D);
        }
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vector2D operator *(Vector2D v, double n)
        {
            return new Vector2D(v.X * n, v.Y * n);
        }
        public static Vector2D operator *(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2D operator /(Vector2D v, double n)
        {
            return new Vector2D(v.X / n, v.Y / n);
        }
        public static Vector2D operator /(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static implicit operator Vector2D(Vector2F vec)
        {
            return new Vector2D(vec.X, vec.Y);
        }
        public static explicit operator Vector2I(Vector2D vec)
        {
            return new Vector2I((int)vec.X, (int)vec.Y);
        }
        public static implicit operator OpenTK.Vector2(Vector2D vec)
        {
            return new OpenTK.Vector2((float)vec.X, (float)vec.Y);
        }
        #endregion
    }
}
