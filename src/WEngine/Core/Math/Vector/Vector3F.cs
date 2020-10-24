using System;
using System.ComponentModel;

namespace WEngine
{
    /// <summary>
    /// A three dimensional <see cref="float"/> vector.
    /// </summary>
    [Serializable]
    public struct Vector3F : IComparable, IComparable<Vector3F>, IEquatable<Vector3F>, IFormattable
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

        #region Multi Dimensional Accessors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector2F XY
        {
            get
            {
                return new Vector2F(this.X, this.Y);
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
                return new Vector2F(this.Y, this.X);
            }

            set
            {
                this.Y = value.X;
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

        #endregion

        #region Directions

        /// <summary>
        /// Positive X
        /// </summary>
        public static Vector3F Right
        {
            get
            {
                return new Vector3F(1.0F, 0.0F, 0.0F);
            }
        }
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector3F Left
        {
            get
            {
                return new Vector3F(-1.0F, 0.0F, 0.0F);
            }
        }

        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector3F Up
        {
            get
            {
                return new Vector3F(0.0F, 1.0F, 0.0F);
            }
        }
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector3F Down
        {
            get
            {
                return new Vector3F(0.0F, -1.0F, 0.0F);
            }
        }

        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3F Forward
        {
            get
            {
                return new Vector3F(0.0F, 0.0F, 1.0F);
            }
        }
        /// <summary>
        /// Negative Z
        /// </summary>
        public static Vector3F Backward
        {
            get
            {
                return new Vector3F(0.0F, 0.0F, -1.0F);
            }
        }

        #endregion

        /// <summary>
        /// All zero
        /// </summary>
        public static Vector3F Zero
        {
            get
            {
                return new Vector3F(0.0F);
            }
        }

        /// <summary>
        /// All postive
        /// </summary>
        public static Vector3F One
        {
            get
            {
                return new Vector3F(1.0F);
            }
        }


        public float SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            }
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(this.SquaredLength);
            }
        }

        public Vector3F Normalized
        {
            get
            {
                return NormalizeVector3F(this);
            }
        }
        #endregion

        #region Constructors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F(Vector2F xy, float z)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = z;

            this.Dimensions = 3;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3F(float x, Vector2F yz)
        {
            this._X = x;
            this._Y = yz.X;
            this._Z = yz.Y;

            this.Dimensions = 3;
        }
        public Vector3F(float values)
        {
            this._X = values;
            this._Y = values;
            this._Z = values;

            this.Dimensions = 3;
        }
        public Vector3F(float x, float y)
        {
            this._X = x;
            this._Y = y;
            this._Z = 0.0F;

            this.Dimensions = 3;
        }
        public Vector3F(float x, float y, float z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;

            this.Dimensions = 3;
        }

        public Vector3F(float[] values)
        {
            this._X = values[0];
            this._Y = values[1];
            this._Z = values[2];

            this.Dimensions = 3;
        }
        #endregion

        #region Methods

        public float[] ToArray()
        {
            return new float[3] {_X, _Y, _Z};
        }
        
        /*public Vector<float> ToSIMD()
        {
            float[] buffer = new float[Vector<float>.Count];
            buffer[0] = _X;
            buffer[1] = _Y;
            buffer[2] = _Z;

            return new Vector<float>(buffer);
        }*/
        
        public static float Distance(Vector3F v1, Vector3F v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static float SquaredDistance(Vector3F v1, Vector3F v2)
        {
            return Math.Abs((v1 - v2).SquaredLength);
        }
        public Vector3F RotateAround(Vector3F pivot, Vector3F axis, float angle)
        {
            return this.RotateAround(pivot, axis * angle);
        }
        public Vector3F RotateAround(Vector3F pivot, Vector3F eulers)
        {
            return this.RotateAround(pivot, new Quaternion(eulers));
        }
        public Vector3F RotateAround(Vector3F pivot, Quaternion rotation)
        {
            return (rotation * (this - pivot)) + pivot;
        }
        public static Vector3F Cross(Vector3F v1, Vector3F v2)
        {
            return new Vector3F
                (v1.Y * v2.Z - v1.Z * v2.Y,
                 v1.Z * v2.X - v1.X * v2.Z,
                 v1.X * v2.Y - v1.Y * v2.X);
        }
        public static Vector3F Dot(Vector3F v1, Vector3F v2)
        {
            return v1 * v2;
        }
        public static float Angle(Vector3F v1, Vector3F v2)
        {
            return (float)Math.Acos((v1.Normalized * v2.Normalized).Length);
        }
        public Vector3F Normalize()
        {
            return this = NormalizeVector3F(this);
        }

        private static Vector3F NormalizeVector3F(Vector3F vector)
        {
            if (vector == Vector3F.Zero)
                return vector;

            return vector / vector.Length;
        }
        public override bool Equals(object obj)
        {
            return obj is Vector3F d &&
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
            return $"Vector3F({this.X.ToString()};{this.Y.ToString()};{this.Z.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector3F({this.X.ToString(provider)};{this.Y.ToString(provider)};{this.Z.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector3F({this.X.ToString(format)};{this.ToString(format)};{this.Z.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector3F({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)};{this.Z.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector3F v ? CompareTo(v) : 1;
        }
        public int CompareTo(Vector3F value)
        {
            float l1 = this.SquaredLength, l2 = value.SquaredLength;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector3F o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y &&
                this.Z == o.Z;
        }

        #endregion

        #region Operators
        public static bool operator ==(Vector3F v1, Vector3F v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }
        public static bool operator !=(Vector3F v1, Vector3F v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
        }
        public static Vector3F operator +(Vector3F v, float n)
        {
            return new Vector3F(v.X + n, v.Y + n, v.Z + n);
        }
        public static Vector3F operator +(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3F operator -(Vector3F v)
        {
            return new Vector3F(v.X * -1.0F, v.Y * -1.0F, v.Z * -1.0F);
        }
        public static Vector3F operator -(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3F operator *(Vector3F v, float n)
        {
            /*if (Engine.IsSIMDAvailable)
            {*/
                /*Vector<float> vn = new Vector<float>(n);

                float[] buffer = new float[Vector<float>.Count];
                (v.ToSIMD() * vn).CopyTo(buffer);
                return new Vector3F(buffer);*/
            /*}
            else
            {*/
                return new Vector3F(v._X * n, v._Y * n, v._Z * n);
            /*}*/
        }
        public static Vector3F operator *(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static Vector3F operator /(Vector3F v, float n)
        {
            return new Vector3F(v.X / n, v.Y / n, v.Z / n);
        }
        public static Vector3F operator /(Vector3F v1, Vector3F v2)
        {
            return new Vector3F(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        public static implicit operator Vector3F(Vector3D vec)
        {
            return new Vector3F((float)vec.X, (float)vec.Y, (float)vec.Z);
        }

        public static implicit operator OpenTK.Vector3(Vector3F vec)
        {
            return new OpenTK.Vector3(vec.X, vec.Y, vec.Z);
        }
        #endregion
    }
}
