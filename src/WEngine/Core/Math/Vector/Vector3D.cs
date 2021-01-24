using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace WEngine
{
    /// <summary>
    /// A three dimensional <see cref="double"/> vector.
    /// </summary>
    [Serializable]
    public struct Vector3D : IVectorable, ICollider, IComparable, IComparable<Vector3D>, IEquatable<Vector3D>, IFormattable
    {
        [JsonIgnore]
        public int Dimensions { get; }

        #region Properties
        [JsonIgnore]
        private double _X;
        public double X
        {
            get => _X;
            set => _X = value;
        }
        [JsonIgnore]
        private double _Y;
        public double Y
        {
            get => _Y;
            set => _Y = value;
        }
        [JsonIgnore]
        private double _Z;
        public double Z
        {
            get => _Z;
            set => _Z = value;
        }

        [JsonIgnore]
        public double this[int index]
        {
            get
            {
                switch(index)
                {
                    case 0:
                        return X;

                    case 1:
                        return Y;

                    case 2:
                        return Z;

                    default:
                        throw new ArgumentOutOfRangeException("Vector3D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z)");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;

                    case 1:
                        Y = value;
                        break;

                    case 2:
                        Z = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Vector3D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z)");
                }
            }
        }


        #region Multi Dimensional Accessors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
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
        public static Vector3D One => new Vector3D(1.0D);

        [JsonIgnore]
        public double SquaredLength => X*X + Y*Y + Z*Z;

        [JsonIgnore]
        public double Length => Math.Sqrt(SquaredLength);

        [JsonIgnore]
        public Vector3D Normalized => NormalizeVector3D(this);

        #endregion

        #region Constructors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3D(Vector2D xy, double z)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = z;

            this.Dimensions = 3;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector3D(double x, Vector2D yz)
        {
            this._X = x;
            this._Y = yz.X;
            this._Z = yz.Y;

            this.Dimensions = 3;
        }
        public Vector3D(double values)
        {
            this._X = values;
            this._Y = values;
            this._Z = values;

            this.Dimensions = 3;
        }
        public Vector3D(double x, double y)
        {
            this._X = x;
            this._Y = y;
            this._Z = 0.0D;

            this.Dimensions = 3;
        }
        [JsonConstructor]
        public Vector3D(double x, double y, double z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;

            this.Dimensions = 3;
        }
        #endregion

        #region Methods
        public static double Distance(Vector3D v1, Vector3D v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static double SquaredDistance(Vector3D v1, Vector3D v2)
        {
            return Math.Abs((v1 - v2).SquaredLength);
        }
        public Vector3D RotateAround(Vector3D pivot, Vector3D axis, float angle)
        {
            return this.RotateAround(pivot, axis * angle);
        }
        public Vector3D RotateAround(Vector3D pivot, Vector3D eulers)
        {
            return this.RotateAround(pivot, new Quaternion(eulers));
        }
        public Vector3D RotateAround(Vector3D pivot, Quaternion rotation)
        {
            return (rotation * (this - pivot)) + pivot;
        }
        public static Vector3D Cross(Vector3D v1, Vector3D v2)
        {
            return new Vector3D
                (v1.Y * v2.Z - v1.Z * v2.Y,
                 v1.Z * v2.X - v1.X * v2.Z,
                 v1.X * v2.Y - v1.Y * v2.X);
        }
        public static Vector3D Dot(Vector3D v1, Vector3D v2)
        {
            return v1 * v2;
        }
        public static double Angle(Vector3D v1, Vector3D v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length) * WMath.RadToDeg;
        }

        public static double SignedAngle(Vector3D from, Vector3D to, Vector3D axis)
        {
            double num1 = Vector3D.Angle(from, to);
            double num2 = from.Y * to.Z - from.Z * to.Y;
            double num3 = from.Z * to.X - from.X * to.Z;
            double num4 = from.X * to.Y - from.Y * to.X;
            double num5 = Math.Sign(axis.X * num2 + axis.Y * num3 + axis.Z * num4);
            return num1 * num5;
        }

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
            return $"Vector3D({this.X.ToString(format)};{this.Y.ToString(format)};{this.Z.ToString(format)})";
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
            double l1 = this.SquaredLength, l2 = value.SquaredLength;

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
        /*public static Vector3D operator +(Vector3D v, double n)
        {
            return new Vector3D(v.X + n, v.Y + n, v.Z + n);
        }*/
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

        public static implicit operator Vector3D(Vector3F vec)
        {
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }


        
        public static implicit operator OpenTK.Vector3(Vector3D vec)
        {
            return new OpenTK.Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        }
        #endregion
    }
}
