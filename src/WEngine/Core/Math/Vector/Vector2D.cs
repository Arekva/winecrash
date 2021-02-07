using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WEngine
{
    /// <summary>
    /// A 2D Double Vector
    /// </summary>
    [Serializable]
    public struct Vector2D : IComparable, IComparable<Vector2D>, IEquatable<Vector2D>, IFormattable
    {
        private const double RadToDeg = 180.0D / Math.PI;
        
        #region Properties
        
        private double _x; // x attribute for binary serialization
        /// <summary>
        /// X component of this vector.
        /// </summary>
        public double X
        {
            get => _x;
            set => _x = value;
        }
        
        private double _y; // y attribute for binary serialization
        /// <summary>
        /// Y component of this vector.
        /// </summary>
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        #region Multi Dimentional Accessors
        /// <summary>
        /// The X and Y components as a <see cref="Vector2D"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
        public Vector2D XY
        {
            get => this;
            set => this = value;
        }
        /// <summary>
        /// The Y and X components as a <see cref="Vector2D"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
        public Vector2D YX
        {
            get => new Vector2D(Y, X);
            
            set
            {
                Y = value.X;
                X = value.Y;
            }
        }
        #endregion

        /// <summary>
        /// Right direction (1.0, 0.0)
        /// </summary>
        public static Vector2D Right => new Vector2D(1.0D, 0.0D);

        /// <summary>
        /// Left direction (-1.0, 0.0)
        /// </summary>
        public static Vector2D Left => new Vector2D(-1.0D, 0.0D);

        /// <summary>
        /// Up direction (0.0, 1.0)
        /// </summary>
        public static Vector2D Up => new Vector2D(0.0D, 1.0D);
            
        /// <summary>
        /// Down direction (0.0, -1.0)
        /// </summary>
        public static Vector2D Down => new Vector2D(0.0D, -1.0D);
        
        /// <summary>
        /// Null (mathematically) vector (0.0, 0.0)
        /// </summary>
        public static Vector2D Zero => new Vector2D(0.0D);
            
        /// <summary>
        /// Unit vector (1.0, 1.0)
        /// </summary>
        public static Vector2D One => new Vector2D(1.0D);

        /// <summary>
        /// The squared length of this vector. Faster than <see cref="Length"/> but has to be rooted.
        /// </summary>
        [JsonIgnore]
        public double SquaredLength => X*X + Y*Y;
        
        /// <summary>
        /// The length of this vector. Slower than <see cref="Length"/> but rooted.
        /// </summary>
        [JsonIgnore]
        public double Length => Math.Sqrt(this.SquaredLength);

        /// <summary>
        /// The directional version of this vector (1.0 of length)
        /// </summary>
        [JsonIgnore]
        public Vector2D Normalized => NormalizeVector2D(this);
        #endregion

        #region Constructors

        /// <summary>
        /// Create a vector having its X and Y being the input value.
        /// </summary>
        /// <param name="values">The X and Y values.</param>
        public Vector2D(double values) => (_x, _y) = (values, values);

        /// <summary>
        /// Create a vector from X and Y values.
        /// </summary>
        /// <param name="x">The X value.</param>
        /// <param name="y">The Y value.</param>
        [JsonConstructor]
        public Vector2D(double x, double y) => (_x, _y) = (x, y);
        #endregion

        #region Methods
        /// <summary>
        /// Gives the distance between two vectors. Slower than <see cref="SquaredDistance"/>.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The distance between the two vectors (always greater than 0).</returns>
        public static double Distance(Vector2D v1, Vector2D v2) => Math.Abs((v1 - v2).Length);
        
        /// <summary>
        /// Gives the squared distance between two vectors. Faster than <see cref="Distance"/>.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The squared distance between the two vectors (always greater than 0).</returns>
        public static double SquaredDistance(Vector2D v1, Vector2D v2) => Math.Abs((v1 - v2).SquaredLength);
        
        /// <summary>
        /// The dot product (angle) of two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The dot product result</returns>
        public static double Dot(Vector2D v1, Vector2D v2) => v1.X*v2.X + v1.Y*v2.Y;
        
        /// <summary>
        /// The angle between two vectors in degree, ranged in 0°..180°
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The angle between two vectors, in degree.</returns>
        public static double Angle(Vector2D from, Vector2D to) => Math.Acos((from.Normalized * to.Normalized).Length) * RadToDeg;

        /// <summary>
        /// The angle between two vectors in degree, ranged in -180°..180°
        /// </summary>
        /// <param name="from">The first vector.</param>
        /// <param name="to">The second vector.</param>
        /// <returns>The signed angle between two vectors, in degree. (</returns>
        public static double SignedAngle(Vector2D from, Vector2D to) => Vector2D.Angle(from, to) * Math.Sign(from.X * to.Y - from.Y * to.X);

        /// <summary>
        /// Normalize this vector.
        /// </summary>
        /// <returns>This vector but normalized.</returns>
        public Vector2D Normalize() => this = NormalizeVector2D(this);

        /// <summary>
        /// Normalize a 2D vector.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        private static Vector2D NormalizeVector2D(Vector2D vector) => vector.IsInfinity || vector.IsNaN || vector == Zero ? vector : vector / vector.Length;

        public bool ContainsValue(double value) => X == value || Y == value;
        public bool IsNaN => ContainsValue(double.NaN);
        public bool IsNegativeInfinity => ContainsValue(double.NegativeInfinity);
        public bool IsPositiveInfinity => ContainsValue(double.PositiveInfinity);
        public bool IsInfinity => IsNegativeInfinity || IsPositiveInfinity;
        
        public override bool Equals(object obj) => obj is Vector2D v && Equals(v);
        public bool Equals(Vector2D o) => X == o.X && Y == o.Y;

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        #region ToString
        public override string ToString() => $"Vector2D({X.ToString()};{Y.ToString()})";
        
        public string ToString(IFormatProvider provider)=>$"Vector2D({X.ToString(provider)};{Y.ToString(provider)})";
        public string ToString(string format) => $"Vector2D({X.ToString(format)};{ToString(format)})";
        
        public string ToString(string format, IFormatProvider provider) => $"Vector2D({X.ToString(format, provider)};{Y.ToString(format, provider)})";
        #endregion

        public int CompareTo(object value) => value is Vector2D v ? CompareTo(v) : 1;
        public int CompareTo(Vector2D value)
        {
            double l1 = Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Get if two 2D vectors are equal.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>If the vectors are equal.</returns>
        public static bool operator ==(Vector2D v1, Vector2D v2) => v1.Equals(v2);

        /// <summary>
        /// Get if two 2D vectors are not equal.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>If the vectors are not equal.</returns>
        public static bool operator !=(Vector2D v1, Vector2D v2) => !v1.Equals(v2);
        /// <summary>
        /// Add a double to a 2D vector. together.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value to add with.</param>
        /// <returns>The added vector.</returns>
        public static Vector2D operator +(Vector2D v, double n) => new Vector2D(v.X + n, v.Y + n);
        
        /// <summary>
        /// Add two 2D vectors together.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The added vector.</returns>
        public static Vector2D operator +(Vector2D v1, Vector2D v2) => new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        
        /// <summary>
        /// Negates a vector (*-1).
        /// </summary>
        /// <param name="v1">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector2D operator -(Vector2D v) => v * -1.0D;
        
        /// <summary>
        /// Substract a 2D vector by another one.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The substracted vector.</returns>
        public static Vector2D operator -(Vector2D v1, Vector2D v2) => new Vector2D(v1.X - v2.X, v1.Y - v2.Y);

        /// <summary>
        /// Multiply a 2D vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2D operator *(Vector2D v, double n) => new Vector2D(v.X * n, v.Y * n);

        /// <summary>
        /// Multiply two 2D vectors together.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2D operator *(Vector2D v1, Vector2D v2) => new Vector2D(v1.X * v2.X, v1.Y * v2.Y);

        /// <summary>
        /// Divides a 2D vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The divided vector.</returns>
        public static Vector2D operator /(Vector2D v, double n) => new Vector2D(v.X / n, v.Y / n);

        /// <summary>
        /// Divides a 2D vector by another one.
        /// </summary>
        /// <param name="v1">The vector.</param>
        /// <param name="v2">The value.</param>
        /// <returns>The divided vector.</returns>
        public static Vector2D operator /(Vector2D v1, Vector2D v2) => new Vector2D(v1.X / v2.X, v1.Y / v2.Y);
        #endregion
    }
}
