﻿using System;
using System.ComponentModel;

namespace WEngine
{
    /// <summary>
    /// Four dimensionnal <see cref="double"/> Vector.
    /// </summary>
    [Serializable]
    public struct Vector4D : IVectorable, IComparable, IComparable<Vector4D>, IEquatable<Vector4D>, IFormattable
    {
        public int Dimensions { get; }

        #region Properties
        private double _X;
        public double X
        {
            get => _X;
            set => _X = value;
        }
        private double _Y;
        public double Y
        {
            get => _Y;
            set => _Y = value;
        }
        private double _Z;
        public double Z
        {
            get => _Z;
            set => _Z = value;
        }
        private double _W;
        public double W
        {
            get => _W;
            set => _W = value;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;

                    case 1:
                        return Y;

                    case 2:
                        return Z;

                    case 3:
                        return W;

                    default:
                        throw new ArgumentOutOfRangeException("Vector4D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z, 3:W)");
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

                    case 3:
                        W = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Vector4D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z, 3:W)");
                }
            }
        }

        #region Multi dimensional accessors
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(Vector2D xy, double z, double w)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = z;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(double x, Vector2D yz, double w)
        {
            this._X = x;
            this._Y = yz.X;
            this._Z = yz.Y;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(double x, double y, Vector2D zw)
        {
            this._X = x;
            this._Y = y;
            this._Z = zw.X;
            this._W = zw.Y;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(Vector2D xy, Vector2D zw)
        {
            this._X = xy.X;
            this._Y = xy.Y;
            this._Z = zw.X;
            this._W = zw.Y;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(Vector3D xyz, double w)
        {
            this._X = xyz.X;
            this._Y = xyz.Y;
            this._Z = xyz.Z;
            this._W = w;

            this.Dimensions = 4;
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Vector4D(double x, Vector3D yzw)
        {
            this._X = x;
            this._Y = yzw.X;
            this._Z = yzw.Y;
            this._W = yzw.Z;

            this.Dimensions = 4;
        }
        public Vector4D(double values)
        {
            this._X = values;
            this._Y = values;
            this._Z = values;
            this._W = values;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y)
        {
            this._X = x;
            this._Y = y;
            this._Z = 0.0D;
            this._W = 0.0D;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y, double z)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
            this._W = 0.0D;

            this.Dimensions = 4;
        }
        public Vector4D(double x, double y, double z, double w)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
            this._W = w;

            this.Dimensions = 4;
        }
        #endregion

        #region Methods
        public static double Distance(Vector4D v1, Vector4D v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static Vector4D Dot(Vector4D v1, Vector4D v2)
        {
            return v1 * v2;
        }
        public static double Angle(Vector4D v1, Vector4D v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length);
        }
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

        public static implicit operator Vector4D(Vector4F vec)
        {
            return new Vector4D(vec.X, vec.Y, vec.Z, vec.W);
        }
        #endregion
    }
}