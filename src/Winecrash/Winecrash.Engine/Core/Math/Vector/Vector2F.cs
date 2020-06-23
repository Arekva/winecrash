using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Vector2F : IComparable, IComparable<Vector2F>, IEquatable<Vector2F>, IFormattable
    {
        public int Dimensions { get; }


        #region Properties
        public float X { get; set; }
        public float Y { get; set; }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F XY
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
        public Vector2F YX
        {
            get
            {
                return new Vector2F(this.Y, this.X);
            }

            set
            {
                this.Y = value.X;
                this.X = value.Y;
            }
        }

        public static Vector2F Right
        {
            get
            {
                return new Vector2F(1.0F, 0.0F);
            }
        }
        public static Vector2F Left
        {
            get
            {
                return new Vector2F(-1.0F, 0.0F);
            }
        }
        public static Vector2F Up
        {
            get
            {
                return new Vector2F(0.0F, 1.0F);
            }
        }
        public static Vector2F Down
        {
            get
            {
                return new Vector2F(0.0F, -1.0F);
            }
        }

        public static Vector2F Zero
        {
            get
            {
                return new Vector2F(0.0F);
            }
        }
        public static Vector2F One
        {
            get
            {
                return new Vector2F(1.0F);
            }
        }

        public float SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector2F Normalized
        {
            get
            {
                return NormalizeVector2F(this);
            }
        }
        #endregion

        #region Constructors
        public Vector2F(float values)
        {
            this.X = values;
            this.Y = values;

            this.Dimensions = 2;
        }
        public Vector2F(float x, float y)
        {
            this.X = x;
            this.Y = y;

            this.Dimensions = 2;
        }
        #endregion

        #region Methods
        public Vector2F Normalize()
        {
            return this = NormalizeVector2F(this);
        }

        private static Vector2F NormalizeVector2F(Vector2F vector)
        {
            if (vector == Vector2F.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2F d &&
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
            return $"Vector2F({this.X.ToString()};{this.Y.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector2F({this.X.ToString(provider)};{this.Y.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector2F({this.X.ToString(format)};{this.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector2F({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector2F v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector2F value)
        {
            float l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector2F o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector2F v1, Vector2F v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        public static bool operator !=(Vector2F v1, Vector2F v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }
        public static Vector2F operator +(Vector2F v, float n)
        {
            return new Vector2F(v.X + n, v.Y + n);
        }
        public static Vector2F operator +(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2F operator -(Vector2F v)
        {
            return new Vector2F(v.X * -1.0F, v.Y * -1.0F);
        }
        public static Vector2F operator -(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vector2F operator *(Vector2F v, float n)
        {
            return new Vector2F(v.X * n, v.Y * n);
        }
        public static Vector2F operator *(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Vector2F operator /(Vector2F v, float n)
        {
            return new Vector2F(v.X / n, v.Y / n);
        }
        public static Vector2F operator /(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static implicit operator Vector2F(Vector2D vec)
        {
            return new Vector2F((float)vec.X, (float)vec.Y);
        }
        #endregion
    }
}
