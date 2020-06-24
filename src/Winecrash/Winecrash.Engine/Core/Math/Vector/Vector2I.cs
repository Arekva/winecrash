using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Vector2I : IComparable, IComparable<Vector2I>, IEquatable<Vector2I>, IFormattable
    {
        public int Dimensions { get; }

        #region Properties
        public int X { get; set; }
        public int Y { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I XY
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
        public Vector2I YX
        {
            get
            {
                return new Vector2I(this.Y, this.X);
            }

            set
            {
                this.Y = value.X;
                this.X = value.Y;
            }
        }

        public static Vector2I Right
        {
            get
            {
                return new Vector2I(1, 0);
            }
        }
        public static Vector2I Left
        {
            get
            {
                return new Vector2I(-1, 0);
            }
        }
        public static Vector2I Up
        {
            get
            {
                return new Vector2I(0, 1);
            }
        }
        public static Vector2I Down
        {
            get
            {
                return new Vector2I(0, -1);
            }
        }

        public static Vector2I Zero
        {
            get
            {
                return new Vector2I(0);
            }
        }
        public static Vector2I One
        {
            get
            {
                return new Vector2I(1);
            }
        }

        public int SquaredLength
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

        public Vector2I Normalized
        {
            get
            {
                return NormalizeVector2I(this);
            }
        }
        #endregion

        #region Constructors
        public Vector2I(int values)
        {
            this.X = values;
            this.Y = values;

            this.Dimensions = 2;
        }
        public Vector2I(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.Dimensions = 2;
        }
        #endregion

        #region Methods
        public static Vector2I Dot(Vector2I v1, Vector2I v2)
        {
            return v1 * v2;
        }
        public Vector2I Normalize()
        {
            return this = NormalizeVector2I(this);
        }

        private static Vector2I NormalizeVector2I(Vector2I vector)
        {
            if (vector == Vector2I.Zero)
                return vector;

            return vector / (int)vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2I d &&
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
            return $"Vector2I({this.X.ToString()};{this.Y.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector2I({this.X.ToString(provider)};{this.Y.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector2I({this.X.ToString(format)};{this.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector2I({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector2I v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector2I value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector2I o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }
        public static Vector2I operator +(Vector2I v, int n)
        {
            return new Vector2I(v.X + n, v.Y + n);
        }
        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2I operator -(Vector2I v)
        {
            return new Vector2I(v.X * -1, v.Y * -1);
        }
        public static Vector2I operator -(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vector2I operator *(Vector2I v, int n)
        {
            return new Vector2I(v.X * n, v.Y * n);
        }
        public static Vector2I operator *(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2I operator /(Vector2I v, int n)
        {
            return new Vector2I(v.X / n, v.Y / n);
        }
        public static Vector2I operator /(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static implicit operator Vector2I(Vector2F vec)
        {
            return new Vector2I((int)vec.X, (int)vec.Y);
        }

        public static implicit operator Vector2F(Vector2I vec)
        {
            return new Vector2I(vec.X, vec.Y);
        }
        #endregion
    }
}
