using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;

namespace WEngine
{
    /// <summary>
    /// A three dimensional <see cref="double"/> vector.
    /// </summary>
    [Serializable] public struct Vector3D : ICollider, IComparable, IComparable<Vector3D>, IEquatable<Vector3D>, IFormattable
    {
        #region Properties
        [JsonIgnore] private double _x;
        public double X
        {
            get => _x;
            set => _x = value;
        }
        [JsonIgnore] private double _y;
        public double Y
        {
            get => _y;
            set => _y = value;
        }
        [JsonIgnore] private double _z;
        public double Z
        {
            get => _z;
            set => _z = value;
        }

        [JsonIgnore] public double this[int index]
        {
            get => index switch 
            { 
                0 => X, 1 => Y, 2 => Z, 
                _ => throw new ArgumentOutOfRangeException(nameof(index), "Vector3D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z)")
            };


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

                    default: throw new ArgumentOutOfRangeException(nameof(index), "Vector3D indexer must be between 0 and 2 (0:X, 1:Y, 2:Z)");
                }
            }
        }

        #region Multi Dimensional Accessors
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D XY
        {
            get => new Vector2D(this.X, this.Y);
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D YX
        {
            get => new Vector2D(this.Y, this.X);
            set
            {
                this.Y = value.X;
                this.X = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D YZ
        {
            get => new Vector2D(this.Y, this.Z);
            set
            {
                this.Y = value.X;
                this.Z = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D ZY
        {
            get => new Vector2D(this.Z, this.Y);
            set
            {
                this.Z = value.X;
                this.Y = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D XZ
        {
            get => new Vector2D(this.X, this.Z);
            set
            {
                this.X = value.X;
                this.Z = value.Y;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Advanced)][JsonIgnore] public Vector2D ZX
        {
            get => new Vector2D(this.Z, this.X);
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
        public static Vector3D Right => new Vector3D(1.0D, 0.0D, 0.0D);
        /// <summary>
        /// Negative X
        /// </summary>
        public static Vector3D Left => new Vector3D(-1.0D, 0.0D, 0.0D);
        /// <summary>
        /// Positive Y
        /// </summary>
        public static Vector3D Up => new Vector3D(0.0D, 1.0D, 0.0D);
        /// <summary>
        /// Negative Y
        /// </summary>
        public static Vector3D Down => new Vector3D(0.0D, -1.0D, 0.0D);
        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3D Forward => new Vector3D(0.0D, 0.0D, 1.0D);
        /// <summary>
        /// Positive Z
        /// </summary>
        public static Vector3D Backward => new Vector3D(0.0D, 0.0D, -1.0D);
        /// <summary>
        /// All zero
        /// </summary>
        public static Vector3D Zero => new Vector3D(0.0D);
        /// <summary>
        /// All postive
        /// </summary>
        public static Vector3D One => new Vector3D(1.0D);
        #endregion
        
        [JsonIgnore] public double SquaredLength => X*X + Y*Y + Z*Z;
        [JsonIgnore] public double Length => Math.Sqrt(SquaredLength);

        [JsonIgnore] public Vector3D Normalized => NormalizeVector3D(this);
        #endregion

        #region Constructors
        [EditorBrowsable(EditorBrowsableState.Advanced)] public Vector3D(Vector2D xy, double z) => (_x, _y, _z) = (xy.X,xy.Y,z);
        [EditorBrowsable(EditorBrowsableState.Advanced)] public Vector3D(double x, Vector2D yz) => (_x, _y, _z) = (x, yz.X, yz.Y);
        public Vector3D(double values) => (_x, _y, _z) = (values, values, values);
        public Vector3D(double x, double y) => (_x, _y, _z) = (x, y, 0.0);
        [JsonConstructor] public Vector3D(double x, double y, double z) => (_x, _y, _z) = (x, y, z);
        #endregion

        #region Methods
        public static double Distance(Vector3D v1, Vector3D v2) => Math.Abs((v1 - v2).Length);
        public static double SquaredDistance(Vector3D v1, Vector3D v2) => Math.Abs((v1 - v2).SquaredLength);
        public Vector3D RotateAround(Vector3D pivot, Vector3D axis, float angle) => RotateAround(pivot, axis * angle);
        public Vector3D RotateAround(Vector3D pivot, Vector3D eulers) => RotateAround(pivot, new Quaternion(eulers));
        public Vector3D RotateAround(Vector3D pivot, Quaternion rotation) => (rotation * (this - pivot)) + pivot;
        public static Vector3D Cross(Vector3D v1, Vector3D v2) => new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        public static Vector3D Dot(Vector3D v1, Vector3D v2) => v1 * v2;
        public static double Angle(Vector3D v1, Vector3D v2) => Math.Acos((v1.Normalized * v2.Normalized).Length) * WMath.RadToDeg;
        public static Vector3D Lerp(Vector3D a, Vector3D b, double t) => (1.0D - t) * a + t * b;
        public static double SignedAngle(Vector3D from, Vector3D to, Vector3D axis)
        {
            double num1 = Vector3D.Angle(from, to);
            double num2 = from.Y * to.Z - from.Z * to.Y;
            double num3 = from.Z * to.X - from.X * to.Z;
            double num4 = from.X * to.Y - from.Y * to.X;
            double num5 = Math.Sign(axis.X * num2 + axis.Y * num3 + axis.Z * num4);
            return num1 * num5;
        }
        public Vector3D Normalize() => this = NormalizeVector3D(this);
        private static Vector3D NormalizeVector3D(Vector3D vector)
        {
            if (vector == Vector3D.Zero)
                return vector;

            return vector / vector.Length;
        }
        
        public override bool Equals(object obj) => obj is Vector3D v && Equals(v);
        public bool Equals(Vector3D o) => X == o.X && Y == o.Y && Z == o.Z;
        
        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + this.X.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            hashCode = hashCode * -1521134295 + this.Z.GetHashCode();
            return hashCode;
        }
        
        
        #region ToString
        public override string ToString() => $"Vector3D({X.ToString()};{Y.ToString()};{Z.ToString()})";
        public string ToString(IFormatProvider provider) => $"Vector3D({X.ToString(provider)};{Y.ToString(provider)};{this.Z.ToString(provider)})";
        public string ToString(string format) => $"Vector3D({X.ToString(format)};{Y.ToString(format)};{Z.ToString(format)})";
        public string ToString(string format, IFormatProvider provider) => $"Vector3D({X.ToString(format, provider)};{Y.ToString(format, provider)};{Z.ToString(format, provider)})";
        #endregion

        public int CompareTo(object value) => value is Vector3D v ? CompareTo(v) : 1;
        public int CompareTo(Vector3D value)
        {
            double l1 = this.SquaredLength, l2 = value.SquaredLength;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        #endregion

        #region Operators
        public static bool operator ==(Vector3D v1, Vector3D v2) => v1.Equals(v2);
        public static bool operator !=(Vector3D v1, Vector3D v2) => !v1.Equals(v2);
        public static Vector3D operator +(Vector3D v1, Vector3D v2) => new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        public static Vector3D operator -(Vector3D v) => new Vector3D(v.X * -1.0D, v.Y * -1.0D, v.Z * -1.0D);
        public static Vector3D operator -(Vector3D v1, Vector3D v2) => new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        public static Vector3D operator *(Vector3D v, double n) => new Vector3D(v.X * n, v.Y * n, v.Z * n);
        public static Vector3D operator *(double n, Vector3D v) => v * n;
        public static Vector3D operator *(Vector3D v1, Vector3D v2) => new Vector3D(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        public static Vector3D operator /(Vector3D v, double n) => new Vector3D(v.X / n, v.Y / n, v.Z / n);
        public static Vector3D operator /(Vector3D v1, Vector3D v2) => new Vector3D(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        public static implicit operator Vector3D(Vector3F vec) => new Vector3D(vec.X, vec.Y, vec.Z);
        public static implicit operator OpenTK.Vector3(Vector3D vec) => new OpenTK.Vector3((float)vec.X, (float)vec.Y, (float)vec.Z);
        #endregion
    }
}