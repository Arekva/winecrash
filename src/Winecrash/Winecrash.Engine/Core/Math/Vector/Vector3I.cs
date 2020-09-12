using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WEngine
{
    [Serializable]
    public struct Vector3I : IComparable, IComparable<Vector3I>, IEquatable<Vector3I>, IFormattable
    {
        [JsonIgnore]
        public int Dimensions { get; }

        #region Properties
        [JsonIgnore]
        private int _X;
        public int X
        {
            get => _X;
            set => _X = value;
        }
        [JsonIgnore]
        private int _Y;
        public int Y
        {
            get => _Y;
            set => _Y = value;
        }
        [JsonIgnore]
        private int _Z;
        public int Z
        {
            get => _Z;
            set => _Z = value;
        }

        #region Multi Dimensional Accessors
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I XY
        {
            get
            {
                return new Vector2I(this.X, this.Y);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
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
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I YZ
        {
            get
            {
                return new Vector2I(this.Y, this.Z);
            }

            set
            {
                this.Y = value.X;
                this.Z = value.Y;
            }
        }
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I ZY
        {
            get
            {
                return new Vector2I(this.Z, this.Y);
            }

            set
            {
                this.Z = value.X;
                this.Y = value.Y;
            }
        }
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I XZ
        {
            get
            {
                return new Vector2I(this.X, this.Z);
            }

            set
            {
                this.X = value.X;
                this.Z = value.Y;
            }
        }
        [JsonIgnore]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2I ZX
        {
            get
            {
                return new Vector2I(this.Z, this.X);
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
        public static Vector3I Right
        {
            get
            {
                return new Vector3I(1, 0, 0);
            }
        }
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector3I Left
        {
            get
            {
                return new Vector3I(-1, 0, 0);
            }
        }

        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector3I Up
        {
            get
            {
                return new Vector3I(0, 1, 0);
            }
        }
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector3I Down
        {
            get
            {
                return new Vector3I(0, -1, 0);
            }
        }

        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3I Forward
        {
            get
            {
                return new Vector3I(0, 0, 1);
            }
        }
        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3I Backward
        {
            get
            {
                return new Vector3I(0, 0, -1);
            }
        }

        #endregion

        /// <summary>
        /// All zero
        /// </summary>
        public static Vector3I Zero
        {
            get
            {
                return new Vector3I(0);
            }
        }

        /// <summary>
        /// All postive
        /// </summary>
        public static Vector3I One
        {
            get
            {
                return new Vector3I(1);
            }
        }
        [JsonIgnore]
        public int SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
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
        #endregion

        #region Constructors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3I(Vector2I xy, int z)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = z;

            this.Dimensions = 3;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3I(int x, Vector3I yz)
        {
            this._X = x;
            this._Y = yz.X;
            this._Z = yz.Y;

            this.Dimensions = 3;
        }
        public Vector3I(int values)
        {
            this._X = values;
            this._Y = values;
            this._Z = values;

            this.Dimensions = 3;
        }
        public Vector3I(int x, int y)
        {
            this._X = x;
            this._Y = y;
            this._Z = 0;

            this.Dimensions = 3;
        }
        public Vector3I(int x, int y, int z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;

            this.Dimensions = 3;
        }
        #endregion

        #region Methods
        public static double Distance(Vector3I v1, Vector3I v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static Vector3I Cross(Vector3I v1, Vector3I v2)
        {
            return new Vector3I
                (v1.Y * v2.Z - v1.Z * v2.Y,
                 v1.Z * v2.X - v1.X * v2.Z,
                 v1.X * v2.Y - v1.Y * v2.X);
        }
        public static Vector3I Dot(Vector3I v1, Vector3I v2)
        {
            return v1 * v2;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3I d &&
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
            return $"Vector3I({this.X.ToString()};{this.Y.ToString()};{this.Z.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector3I({this.X.ToString(provider)};{this.Y.ToString(provider)};{this.Z.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector3I({this.X.ToString(format)};{this.ToString(format)};{this.Z.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector3I({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)};{this.Z.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector3I v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector3I value)
        {
            double l1 = this.SquaredLength, l2 = value.SquaredLength;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector3I o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y &&
                this.Z == o.Z;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector3I v1, Vector3I v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }
        public static bool operator !=(Vector3I v1, Vector3I v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }
        public static Vector3I operator +(Vector3I v, int n)
        {
            return new Vector3I(v.X + n, v.Y + n, v.Z + n);
        }
        public static Vector3I operator +(Vector3I v1, Vector3I v2)
        {
            return new Vector3I(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3I operator -(Vector3I v)
        {
            return new Vector3I(v.X * -1, v.Y * -1, v.Z * -1);
        }
        public static Vector3I operator -(Vector3I v1, Vector3I v2)
        {
            return new Vector3I(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3I operator *(Vector3I v, int n)
        {
            return new Vector3I(v.X * n, v.Y * n, v.Z * n);
        }
        public static Vector3I operator *(Vector3I v1, Vector3I v2)
        {
            return new Vector3I(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static Vector3I operator /(Vector3I v, int n)
        {
            return new Vector3I(v.X / n, v.Y / n, v.Z / n);
        }
        public static Vector3I operator /(Vector3I v1, Vector3I v2)
        {
            return new Vector3I(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        public static implicit operator Vector3I(Vector3F vec)
        {
            return new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
        }

        public static implicit operator Vector3I(Vector3D vec)
        {
            return new Vector3I((int)vec.X, (int)vec.Y, (int)vec.Z);
        }

        public static implicit operator Vector3F(Vector3I vec)
        {
            return new Vector3F(vec.X, vec.Y, vec.Z);
        }

        public static implicit operator Vector3D(Vector3I vec)
        {
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }

        public static implicit operator OpenTK.Vector3(Vector3I vec)
        {
            return new OpenTK.Vector3(vec.X, vec.Y, vec.Z);
        }
        #endregion
    }
}
