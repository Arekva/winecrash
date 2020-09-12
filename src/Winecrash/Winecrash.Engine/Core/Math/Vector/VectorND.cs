using System;
namespace WEngine
{
    /// <summary>
    /// A generic <see cref="double"/> undefined dimension vector.
    /// <br>For 2D, 3D and 4D, use <see cref="Vector2D"/>/<see cref="Vector3D"/>/<see cref="Vector4D"/></br>
    /// </summary>
    public struct VectorND : IVectorable, IComparable, IComparable<VectorND>, IEquatable<VectorND>, IFormattable
    {
        public int Dimensions { get; }

        /// <summary>
        /// Get a component 
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public double this[int dimension]
        {
            get
            {
                if (dimension < 1 || dimension > this.Dimensions)
                    throw new ArgumentOutOfRangeException(nameof(dimension), "The dimension must be superior to 0 and inferior or equals to the dimensions amount");

                return Componants[dimension - 1];
            }
            
            set
            {
                if (dimension < 1 || dimension > this.Dimensions)
                    throw new ArgumentOutOfRangeException(nameof(dimension), "The dimension must be superior to 0 and inferior or equals to the dimensions amount");

                Componants[dimension - 1] = value;
            }
        }

        /// <summary>
        /// All the components of this <see cref="double"/> N dimensional vector.
        /// </summary>
        private double[] Componants;

        public double SquaredLength
        {
            get
            {
                double sl = 0.0D;

                for (int i = 0; i < this.Dimensions; i++)
                {
                    double c = this.Componants[i];

                    sl += c * c;
                }

                return sl;
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SquaredLength);
            }
        }

        /// <summary>
        /// Get the direction version of this vector (Length 1.0)
        /// </summary>
        public VectorND Normalized
        {
            get
            {
                return NormalizeVectorND(this);
            }
        }

        
        /// <summary>
        /// Creates a <see cref="double"/> N dimensional vector.
        /// </summary>
        /// <param name="dimensions">The number of dimensions of the vector.</param>
        public VectorND(int dimensions)
        {
            if (dimensions < 1) 
                throw new ArgumentOutOfRangeException(nameof(dimensions), "The amount of dimensions of the VectorND must be superior to 0");

            this.Dimensions = dimensions;

            this.Componants = new double[dimensions];
        }
        /// <summary>
        /// Creates a <see cref="double"/> N dimensional vector with default values.
        /// </summary>
        /// <param name="dimensions">The number of dimensions of the vector.</param>
        /// <param name="values">The values to fill the components with.</param>
        public VectorND(int dimensions, double values)
        {
            if (dimensions < 1)
                throw new ArgumentOutOfRangeException(nameof(dimensions), "The amount of dimensions of the VectorND must be superior to 0");

            this.Dimensions = dimensions;

            this.Componants = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                this.Componants[i] = values;
            }
        }

        /// <summary>
        /// The distance between two N dimensional vectors of the same dimension.
        /// </summary>
        /// <param name="v1">First vector.</param>
        /// <param name="v2">Second vector.</param>
        /// <returns>The distance between both vectors.</returns>
        public static double Distance(VectorND v1, VectorND v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        /// <summary>
        /// The angle between two N dimensional vectors of the same dimension.
        /// </summary>
        /// <param name="v1">First vector.</param>
        /// <param name="v2">Second vector.</param>
        /// <returns>The Angle between both vectors.</returns>
        public static double Angle(VectorND v1, VectorND v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length);
        }
        /// <summary>
        /// Normalize this vector.
        /// </summary>
        /// <returns>This vector normalized.</returns>
        public VectorND Normalize()
        {
            return this = NormalizeVectorND(this);
        }
        /// <summary>
        /// Normalizes an ND vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The normalized vector.</returns>
        private static VectorND NormalizeVectorND(VectorND vector)
        {
            return vector / vector.Length;
        }

        public override bool Equals(object obj)
        {
            if(obj is VectorND d)
            {
                if(d.Dimensions == this.Dimensions)
                {
                    for (int i = 0; i < this.Dimensions; i++)
                        if (this.Componants[i] != d.Componants[i]) 
                            return false;
                    return true;
                }     
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = -307843816;
            for (int i = 0; i < this.Dimensions; i++)
            {
                hashCode *= -1521134295 + this.Componants[i].GetHashCode();
            }
            return hashCode;
        }

        #region ToString
        public override string ToString()
        {
            string txt = $"VectorND[{this.Dimensions}](";

            for (int i = 0; i < this.Dimensions; i++)
            {
                txt = String.Concat(txt, this.Componants[i].ToString());

                if (i < this.Dimensions - 1)
                {
                    txt = String.Concat(txt, ";");
                }
            }

            return txt + ")";
        }
        public string ToString(IFormatProvider provider)
        {
            string txt = $"VectorND[{this.Dimensions}](";

            for (int i = 0; i < this.Dimensions; i++)
            {
                txt = String.Concat(txt, this.Componants[i].ToString(provider));

                if (i < this.Dimensions - 1)
                {
                    txt = String.Concat(txt, ";");
                }
            }

            return txt + ")";
        }
        public string ToString(string format)
        {
            string txt = $"VectorND[{this.Dimensions}](";

            for (int i = 0; i < this.Dimensions; i++)
            {
                txt = String.Concat(txt, this.Componants[i].ToString(format));

                if (i < this.Dimensions - 1)
                {
                    txt = String.Concat(txt, ";");
                }
            }

            return txt + ")";
        }
        public string ToString(string format, IFormatProvider provider)
        {
            string txt = $"VectorND[{this.Dimensions}](";

            for (int i = 0; i < this.Dimensions; i++)
            {
                txt = String.Concat(txt, this.Componants[i].ToString(format, provider));

                if (i < this.Dimensions - 1)
                {
                    txt = String.Concat(txt, ";");
                }
            }

            return txt + ")";
        }

        public int CompareTo(object value)
        {
            return value is VectorND v ? CompareTo(v) : 1;
        }

        public int CompareTo(VectorND value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(VectorND o)
        {
            if (o.Dimensions != this.Dimensions) 
                return false;

            for (int i = 0; i < this.Dimensions; i++)
                if (o.Componants[i] != this.Componants[i])
                    return false;

            return true;
        }
        #endregion


        #region Operators
        public static bool operator ==(VectorND v1, VectorND v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(VectorND v1, VectorND v2)
        {
            return v1.Equals(v2);
        }
        public static VectorND operator +(VectorND v, double n)
        {
            VectorND vec = new VectorND(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] + n;

            return vec;
        }
        public static VectorND operator +(VectorND v1, VectorND v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot add two vector of different dimensions.");

            VectorND vec = new VectorND(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] + v2.Componants[i];

            return vec;
        }
        public static VectorND operator -(VectorND v)
        {
            return v * -1.0F;
        }
        public static VectorND operator -(VectorND v1, VectorND v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot substract two vectors of different dimensions.");

            VectorND vec = new VectorND(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] - v2.Componants[i];

            return vec;
        }
        public static VectorND operator *(VectorND v, double n)
        {
            VectorND vec = new VectorND(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] * n;

            return vec;
        }
        public static VectorND operator *(VectorND v1, VectorND v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot multiply two vectors of different dimensions.");

            VectorND vec = new VectorND(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] - v2.Componants[i];

            return vec;
        }
        public static VectorND operator /(VectorND v, double n)
        {
            VectorND vec = new VectorND(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] / n;

            return vec;
        }
        public static VectorND operator /(VectorND v1, VectorND v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot divide two vectors of different dimensions.");

            VectorND vec = new VectorND(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] / v2.Componants[i];

            return vec;
        }
        #endregion
    }
}
