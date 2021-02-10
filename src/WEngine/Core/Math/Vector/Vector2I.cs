using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WEngine
{
    /// <summary>
    /// A 2D <see cref="int"/> Vector
    /// </summary>
    [Serializable]
    public struct Vector2I : IComparable, IComparable<Vector2I>, IEquatable<Vector2I>, IFormattable
    {
        #region Properties
        /// <summary>
        /// Serialized X component of the vector.
        /// </summary>
        [JsonIgnore]
        private int _x;
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public int X
        {
            get => _x;
            set => _x = value;
        }
        /// <summary>
        /// Serialized Y component of the vector.
        /// </summary>
        [JsonIgnore]
        private int _y;
        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public int Y
        {
            get => _y;
            set => _y = value;
        }

        [JsonIgnore]
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
        [JsonIgnore]
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

        /// <summary>
        /// 2D Right (1,0)
        /// </summary>
        [JsonIgnore]
        public static Vector2I Right
        {
            get
            {
                return new Vector2I(1, 0);
            }
        }
        /// <summary>
        /// 2D Left (-1,0)
        /// </summary>
        [JsonIgnore]
        public static Vector2I Left
        {
            get
            {
                return new Vector2I(-1, 0);
            }
        }
        /// <summary>
        /// 2D Up (1,0)
        /// </summary>
        [JsonIgnore]
        public static Vector2I Up
        {
            get
            {
                return new Vector2I(0, 1);
            }
        }
        /// <summary>
        /// 2D Down (-1,0)
        /// </summary>
        [JsonIgnore]
        public static Vector2I Down
        {
            get
            {
                return new Vector2I(0, -1);
            }
        }

        /// <summary>
        /// Null (mathematically) vector (0,0)
        /// </summary>
        [JsonIgnore]
        public static Vector2I Zero
        {
            get
            {
                return new Vector2I(0);
            }
        }

        /// <summary>
        /// Unit vector (1,1)
        /// </summary>
        [JsonIgnore]
        public static Vector2I One
        {
            get
            {
                return new Vector2I(1);
            }
        }
        [JsonIgnore]
        public double SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        
        [JsonIgnore]
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// The directional version of this vector (length 1)
        /// </summary>
        [JsonIgnore]
        public Vector2I Normalized
        {
            get
            {
                return NormalizeVector2I(this);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create an <see cref="int"/> 2D vector where all the values are the parameter one.
        /// </summary>
        /// <param name="values">The components values.</param>
        public Vector2I(int values)
        {
            _x = values;
            _y = values;
        }
        /// <summary>
        /// Create an <see cref="int"/> 2D vector.
        /// </summary>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        [JsonConstructor]
        public Vector2I(int x, int y)
        {
            _x = x;
            _y = y;
        }
        #endregion

        #region Methods
        /// <summary>
        /// The distance between two <see cref="int"/> 2D vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The distance between both vector. Always positive.</returns>
        public static double Distance(Vector2I v1, Vector2I v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        /// <summary>
        /// The dot product (multiplication) of two <see cref="int"/> 2D vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns></returns>
        public static Vector2I Dot(Vector2I v1, Vector2I v2)
        {
            return v1 * v2;
        }
        /// <summary>
        /// Normalize this <see cref="int"/> 2D vector.
        /// </summary>
        /// <returns>This vector after normalization.</returns>
        public Vector2I Normalize()
        {
            return this = NormalizeVector2I(this);
        }
        /// <summary>
        /// Normalize a <see cref="int"/> 2D vector.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized <see cref="int"/> vector. </returns>
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

        public static implicit operator Vector2I(Vector2D vec)
        {
            return new Vector2I((int)vec.X, (int)vec.Y);
        }
        public static implicit operator Vector2I(Vector2F vec)
        {
            return new Vector2I((int)vec.X, (int)vec.Y);
        }
        public static implicit operator Vector2F(Vector2I vec)
        {
            return new Vector2F(vec.X, vec.Y);
        }

        public static implicit operator Vector2D(Vector2I vec)
        {
            return new Vector2D(vec.X, vec.Y);
        }
        #endregion
    }
}
