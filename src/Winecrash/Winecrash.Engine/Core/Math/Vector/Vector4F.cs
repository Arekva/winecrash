using OpenTK;
using System;
using System.ComponentModel;

namespace WEngine
{
    /// <summary>
    /// Four dimentional <see cref="float"/> vector.
    /// </summary>
    [Serializable]
    public struct Vector4F : IComparable, IComparable<Vector4F>, IEquatable<Vector4F>, IFormattable
    {
        public int Dimensions { get; }

        #region Properties
        private float _X;
        public float X
        {
            get => _X;
            set => _X = value;
        }
        private float _Y;
        public float Y
        {
            get => _Y;
            set => _Y = value;
        }
        private float _Z;
        public float Z
        {
            get => _Z;
            set => _Z = value;
        }
        private float _W;
        public float W
        {
            get => _W;
            set => _W = value;
        }

        #region Multi dimensional accessors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F XY
        {
            get
            {
                return new Vector2F(X, Y);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F YX
        {
            get
            {
                return new Vector2F(Y, X);
            }

            set
            {
                this.Y = value.X;
                this.X = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F XZ
        {
            get
            {
                return new Vector2F(this.X, this.Z);
            }

            set
            {
                this.X = value.X;
                this.Z = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F ZX
        {
            get
            {
                return new Vector2F(this.Z, this.X);
            }

            set
            {
                this.Z = value.X;
                this.X = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F XW
        {
            get
            {
                return new Vector2F(this.X, this.W);
            }

            set
            {
                this.X = value.X;
                this.W = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F WX
        {
            get
            {
                return new Vector2F(this.W, this.X);
            }
            set
            {
                this.W = value.X;
                this.X = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F YZ
        {
            get
            {
                return new Vector2F(this.Y, this.Z);
            }

            set
            {
                this.Y = value.X;
                this.Z = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F ZY
        {
            get
            {
                return new Vector2F(this.Z, this.Y);
            }

            set
            {
                this.Z = value.X;
                this.Y = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F YW
        {
            get
            {
                return new Vector2F(this.Y, this.W);
            }

            set
            {
                this.Y = value.X;
                this.W = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F WY
        {
            get
            {
                return new Vector2F(this.W, this.Y);
            }

            set
            {
                this.W = value.X;
                this.Y = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F ZW
        {
            get
            {
                return new Vector2F(this.Z, this.W);
            }

            set
            {
                this.Z = value.X;
                this.W = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F WZ
        {
            get
            {
                return new Vector2F(this.W, this.Z);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F XYZ
        {
            get
            {
                return new Vector3F(this.X, this.Y, this.Z);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F ZYX
        {
            get
            {
                return new Vector3F(this.Z, this.Y, this.X);
            }

            set
            {
                this.Z = value.X;
                this.Y = value.Y;
                this.X = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F YZW
        {
            get
            {
                return new Vector3F(this.Y, this.Z, this.W);
            }

            set
            {
                this.Y = value.X;
                this.Z = value.Y;
                this.W = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F WZY
        {
            get
            {
                return new Vector3F(this.W, this.Z, this.Y);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
                this.Y = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F XZW
        {
            get
            {
                return new Vector3F(this.X, this.Z, this.W);
            }

            set
            {
                this.X = value.X;
                this.Z = value.Y;
                this.W = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F WZX
        {
            get
            {
                return new Vector3F(this.W, this.Z, this.X);
            }

            set
            {
                this.W = value.X;
                this.Z = value.Y;
                this.X = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F XYW
        {
            get
            {
                return new Vector3F(this.X, this.Y, this.W);
            }

            set
            {
                this.X = value.X;
                this.Y = value.Y;
                this.W = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F WYZ
        {
            get
            {
                return new Vector3F(this.W, this.Y, this.Z);
            }

            set
            {
                this.W = value.X;
                this.Y = value.Y;
                this.Z = value.Z;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F XYZW
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
        public Vector4F WZYX
        {
            get
            {
                return new Vector4F(this.W, this.Z, this.Y, this.X);
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
        public static Vector4F Right
        {
            get
            {
                return new Vector4F(1.0F, 0.0F, 0.0F, 0.0F);
            }
        }
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector4F Left
        {
            get
            {
                return new Vector4F(-1.0F, 0.0F, 0.0F, 0.0F);
            }
        }
        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector4F Up
        {
            get
            {
                return new Vector4F(0.0F, 1.0F, 0.0F, 0.0F);
            }
        }
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector4F Down
        {
            get
            {
                return new Vector4F(0.0F, -1.0F, 0.0F, 0.0F);
            }
        }
        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector4F Forward
        {
            get
            {
                return new Vector4F(0.0F, 0.0F, 1.0F, 0.0F);
            }
        }
        /// <summary>
        /// Negative Z
        /// </summary>
        public static Vector4F Backward
        {
            get
            {
                return new Vector4F(0.0F, 0.0F, -1.0F, 0.0F);
            }
        }
        /// <summary>
        /// Positive W
        /// </summary>
        public static Vector4F Ana
        {
            get
            {
                return new Vector4F(0.0F, 0.0F, 0.0F, 1.0F);
            }
        }
        /// <summary>
        /// Negative W
        /// </summary>
        public static Vector4F Kata
        {
            get
            {
                return new Vector4F(0.0F, 0.0F, 0.0F, -1.0F);
            }
        }
        #endregion

        /// <summary>
        /// All zero
        /// </summary>
        public static Vector4F Zero
        {
            get
            {
                return new Vector4F(0.0F);
            }
        }
        /// <summary>
        /// All positive
        /// </summary>
        public static Vector4F One
        {
            get
            {
                return new Vector4F(1.0F);
            }
        }

        public float SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            }
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector4F Normalized
        {
            get
            {
                return NormalizeVector4F(this);
            }
        }
        #endregion

        #region Constructors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(Vector2F xy, float z, float w)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = z;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(float x, Vector2F yz, float w)
        {
            this._X = x;
            this._Y = yz.X;
            this._Z = yz.Y;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(float x, float y, Vector2F zw)
        {
            this._X = x;
            this._Y = y;
            this._Z = zw.X;
            this._W = zw.Y;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(Vector2F xy, Vector2F zw)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = zw.X;
            this._W = zw.Y;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(Vector3F xyz, float w)
        {
            this._X = xyz.X;
            this._Y = xyz.Y;
            this._Z = xyz.Z;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4F(float x, Vector3F yzw)
        {
            this._X = x;
            this._Y = yzw.X;
            this._Z = yzw.Y;
            this._W = yzw.Z;

            this.Dimensions = 4;
        }
        public Vector4F(float values)
        {
            this._X = values;
            this._Y = values;
            this._Z = values;
            this._W = values;

            this.Dimensions = 4;
        }
        public Vector4F(float x, float y)
        {
            this._X = x;
            this._Y = y;
            this._Z = 0.0F;
            this._W = 0.0F;

            this.Dimensions = 4;
        }
        public Vector4F(float x, float y, float z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
            this._W = 0.0F;

            this.Dimensions = 4;
        }
        public Vector4F(float x, float y, float z, float w)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
            this._W = w;

            this.Dimensions = 4;
        }
        #endregion

        #region Methods
        public static double Distance(Vector4F v1, Vector4F v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static Vector4F Dot(Vector4F v1, Vector4F v2)
        {
            return v1 * v2;
        }
        public static float Angle(Vector4F v1, Vector4F v2)
        {
            return (float)Math.Acos((v1.Normalized * v2.Normalized).Length);
        }

        public Vector4F Normalize()
        {
            return this = NormalizeVector4F(this);
        }

        private static Vector4F NormalizeVector4F(Vector4F vector)
        {
            if (vector == Vector4F.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4F d &&
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
            return $"Vector4F({this.X.ToString()};{this.Y.ToString()};{this.Z.ToString()};{this.W.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector4F({this.X.ToString(provider)};{this.Y.ToString(provider)};{this.Z.ToString(provider)};{this.W.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector4F({this.X.ToString(format)};{this.ToString(format)};{this.Z.ToString(format)};{this.W.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector4F({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)};{this.Z.ToString(format, provider)};{this.W.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector4F v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector4F value)
        {
            float l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector4F o)
        {
            return 
                this.X == o.X && 
                this.Y == o.Y && 
                this.Z == o.Z &&
                this.W == o.W;
        }

        #endregion

        #region Operators
        public static bool operator == (Vector4F v1, Vector4F v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z && v1.W == v2.W;
        }
        public static bool operator != (Vector4F v1, Vector4F v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z || v1.W != v2.W;
        }
        public static Vector4F operator + (Vector4F v, float n)
        {
            return new Vector4F(v.X + n, v.Y + n, v.Z + n, v.W + n);
        }
        public static Vector4F operator + (Vector4F v1, Vector4F v2)
        {
            return new Vector4F(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }
        public static Vector4F operator - (Vector4F v)
        {
            return new Vector4F(v.X * -1.0F, v.Y * -1.0F, v.Z * -1.0F, v.W * -1.0F);
        }
        public static Vector4F operator - (Vector4F v1, Vector4F v2)
        {
            return new Vector4F(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }
        public static Vector4F operator * (Vector4F v, float n)
        {
            return new Vector4F(v.X * n, v.Y * n, v.Z * n, v.W * n);
        }
        public static Vector4F operator * (Vector4F v1, Vector4F v2)
        {
            return new Vector4F(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }
        public static Vector4F operator /(Vector4F v, float n)
        {
            return new Vector4F(v.X / n, v.Y / n, v.Z / n, v.W / n);
        }
        public static Vector4F operator /(Vector4F v1, Vector4F v2)
        {
            return new Vector4F(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
        }

        public static implicit operator Vector4F(Vector4D vec)
        {
            return new Vector4F((float)vec.X, (float)vec.Y, (float)vec.Z, (float)vec.W);
        }

        public static implicit operator Vector4(Vector4F vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
        #endregion
    }
}


