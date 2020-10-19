using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace WEngine
{
    /// <summary>
    /// A 2D Double Vector
    /// </summary>
    [Serializable]
    public struct Vector2D : IVectorable, IComparable, IComparable<Vector2D>, IEquatable<Vector2D>, IFormattable
    {
        [JsonIgnore]
        public int Dimensions { get; }

        #region Properties
        /// <summary>
        /// Serializable X component of this vector.
        /// </summary>
        [JsonIgnore]
        private double _X;
        /// <summary>
        /// X component of this vector.
        /// </summary>
        public double X
        {
            get => _X;
            set => _X = value;
        }
        /// <summary>
        /// Serializable Y component of this vector.
        /// </summary>
        [JsonIgnore]
        private double _Y;
        /// <summary>
        /// Y component of this vector.
        /// </summary>
        public double Y
        {
            get => _Y;
            set => _Y = value;
        }

#region Multi Dimentional Accessors
        /// <summary>
        /// The X and Y components as a <see cref="Vector2D"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [JsonIgnore]
        public Vector2D XY
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
        /// <summary>
        /// The Y and X components as a <see cref="Vector2D"/>
        /// </summary>
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
        #endregion

        /// <summary>
        /// Right direction (1.0, 0.0)
        /// </summary>
        public static Vector2D Right
        {
            get
            {
                return new Vector2D(1.0D, 0.0D);
            }
        }
        /// <summary>
        /// Left direction (-1.0, 0.0)
        /// </summary>
        public static Vector2D Left
        {
            get
            {
                return new Vector2D(-1.0D, 0.0D);
            }
        }

        /// <summary>
        /// Up direction (0.0, 1.0)
        /// </summary>
        public static Vector2D Up
        {
            get
            {
                return new Vector2D(0.0D, 1.0D);
            }
        }
        /// <summary>
        /// Down direction (0.0, -1.0)
        /// </summary>
        public static Vector2D Down
        {
            get
            {
                return new Vector2D(0.0D, -1.0D);
            }
        }
        /// <summary>
        /// Null (mathematically) vector (0.0, 0.0)
        /// </summary>
        public static Vector2D Zero
        {
            get
            {
                return new Vector2D(0.0D);
            }
        }
        /// <summary>
        /// Unit vector (1.0, 1.0)
        /// </summary>
        public static Vector2D One
        {
            get
            {
                return new Vector2D(1.0D);
            }
        }

        /// <summary>
        /// The squared length of this vector. Faster than <see cref="Length"/> but has to be rooted.
        /// </summary>
        [JsonIgnore]
        public double SquaredLength
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        /// <summary>
        /// The length of this vector. Slower than <see cref="Length"/> but rooted.
        /// </summary>
        [JsonIgnore]
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// The directional version of this vector (1.0 of length)
        /// </summary>
        [JsonIgnore]
        public Vector2D Normalized
        {
            get
            {
                return NormalizeVector2D(this);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a vector having its X and Y being the input value.
        /// </summary>
        /// <param name="values">The X and Y values.</param>
        public Vector2D(double values)
        {
            this._X = values;
            this._Y = values;

            this.Dimensions = 2;
        }
        /// <summary>
        /// Create a vector from X and Y values.
        /// </summary>
        /// <param name="x">The X value.</param>
        /// <param name="y">The Y value.</param>
        [JsonConstructor]
        public Vector2D(double x, double y)
        {
            this._X = x;
            this._Y = y;

            this.Dimensions = 2;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Give the distance between two vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The distance between the two vectors (always greater than 0).</returns>
        public static double Distance(Vector2D v1, Vector2D v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        /// <summary>
        /// The dot value of two vectors (Multiplication of the vectors).
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The dot value </returns>
        public static Vector2D Dot(Vector2D v1, Vector2D v2)
        {
            return v1 * v2;
        }
        /// <summary>
        /// The angle between two vectors in degree. The vectors are automatically normalized.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>The angle between two vectors, in degree.</returns>
        public static double Angle(Vector2D v1, Vector2D v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length) * WMath.RadToDeg;
        }

        /// <summary>
        /// Normalize this vector.
        /// </summary>
        /// <returns>This vector but normalized.</returns>
        public Vector2D Normalize()
        {
            return this = NormalizeVector2D(this);
        }

        /// <summary>
        /// Normalized a 2D vector.
        /// </summary>
        /// <param name="vector">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        private static Vector2D NormalizeVector2D(Vector2D vector)
        {
            if (vector == Vector2D.Zero)
                return vector;

            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2D d &&
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
            return $"Vector2D({this.X.ToString()};{this.Y.ToString()})";
        }
        public string ToString(IFormatProvider provider)
        {
            return $"Vector2D({this.X.ToString(provider)};{this.Y.ToString(provider)})";
        }
        public string ToString(string format)
        {
            return $"Vector2D({this.X.ToString(format)};{this.ToString(format)})";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            return $"Vector2D({this.X.ToString(format, provider)};{this.Y.ToString(format, provider)})";
        }
        #endregion

        public int CompareTo(object value)
        {
            return value is Vector2D v ? CompareTo(v) : 1;
        }

        public int CompareTo(Vector2D value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(Vector2D o)
        {
            return
                this.X == o.X &&
                this.Y == o.Y;
        }

        #endregion

        #region Operators
        /// <summary>
        /// Get if two 2D vectors are equal.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>If the vectors are equal.</returns>
        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.X == v2.X && v1.Y == v2.Y;
        }
        /// <summary>
        /// Get if two 2D vectors are not equal.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>If the vectors are not equal.</returns>
        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y;
        }
        /// <summary>
        /// Add a double to a 2D vector. together.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value to add with.</param>
        /// <returns>The added vector.</returns>
        public static Vector2D operator +(Vector2D v, double n)
        {
            return new Vector2D(v.X + n, v.Y + n);
        }
        /// <summary>
        /// Add two 2D vectors together.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The added vector.</returns>
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        /// <summary>
        /// Negates a vector (*-1).
        /// </summary>
        /// <param name="v1">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector2D operator -(Vector2D v)
        {
            return new Vector2D(v.X * -1.0D, v.Y * -1.0D);
        }

        /// <summary>
        /// Substract a 2D vector by another one.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The substracted vector.</returns>
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Multiply a 2D vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2D operator *(Vector2D v, double n)
        {
            return new Vector2D(v.X * n, v.Y * n);
        }

        /// <summary>
        /// Multiply two 2D vectors together (same as <see cref="Dot(Vector2D, Vector2D)"/>).
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The multiplied vector.</returns>
        public static Vector2D operator *(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X * v2.X, v1.Y * v2.Y);
        }

        /// <summary>
        /// Devides a 2D vector by a value.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The devided vector.</returns>
        public static Vector2D operator /(Vector2D v, double n)
        {
            return new Vector2D(v.X / n, v.Y / n);
        }

        /// <summary>
        /// Devides a 2D vector by another one..
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="n">The value.</param>
        /// <returns>The devided vector.</returns>
        public static Vector2D operator /(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X / v2.X, v1.Y / v2.Y);
        }

        /// <summary>
        /// Converts a <see cref="float"/> 2D vector to a <see cref="double"/> 2D vector.
        /// </summary>
        /// <param name="vec">The <see cref="float"/> 2D vector</param>
        public static implicit operator Vector2D(Vector2F vec)
        {
            return new Vector2D(vec.X, vec.Y);
        }

        /// <summary>
        /// Converts a <see cref="double"/> 2D vector to an <see cref="int"/> 2D vector.
        /// </summary>
        /// <param name="vec">The <see cref="double"/> 2D vector</param>
        public static explicit operator Vector2I(Vector2D vec)
        {
            return new Vector2I((int)vec.X, (int)vec.Y);
        }

        /// <summary>
        /// Converts a <see cref="double"/> 2D vector to an OpenTK <see cref="float"/> 2D vector.
        /// </summary>
        /// <param name="vec">The <see cref="double"/> 2D vector</param>
        public static implicit operator OpenTK.Vector2(Vector2D vec)
        {
            return new OpenTK.Vector2((float)vec.X, (float)vec.Y);
        }
        #endregion
    }
}
