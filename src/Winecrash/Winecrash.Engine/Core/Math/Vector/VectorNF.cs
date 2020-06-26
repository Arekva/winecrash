using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;

namespace Winecrash.Engine
{
    public struct VectorNF : IVectorable, IComparable, IComparable<VectorNF>, IEquatable<VectorNF>, IFormattable
    {
        public int Dimensions { get; }

        public float this[int dimension]
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

        private float[] Componants;

        public double SquaredLength
        {
            get
            {
                float sl = 0.0F;

                for (int i = 0; i < this.Dimensions; i++)
                {
                    float c = this.Componants[i];

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

        public VectorNF Normalized
        {
            get
            {
                return NormalizeVectorNF(this);
            }
        }

        

        public VectorNF(int dimensions)
        {
            if (dimensions < 1) 
                throw new ArgumentOutOfRangeException(nameof(dimensions), "The amount of dimensions of the VectorNF must be superior to 0");

            this.Dimensions = dimensions;

            this.Componants = new float[dimensions];
        }
        public VectorNF(int dimensions, float values)
        {
            if (dimensions < 1)
                throw new ArgumentOutOfRangeException(nameof(dimensions), "The amount of dimensions of the VectorNF must be superior to 0");

            this.Dimensions = dimensions;

            this.Componants = new float[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                this.Componants[i] = values;
            }
        }

        public static double Distance(VectorNF v1, VectorNF v2)
        {
            return Math.Abs((v1 - v2).Length);
        }
        public static double Angle(VectorNF v1, VectorNF v2)
        {
            return Math.Acos((v1.Normalized * v2.Normalized).Length);
        }

        public VectorNF Normalize()
        {
            return this = NormalizeVectorNF(this);
        }

        private static VectorNF NormalizeVectorNF(VectorNF vector)
        {
            return vector / (float)vector.Length;
        }

        public override bool Equals(object obj)
        {
            if(obj is VectorNF d)
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
            string txt = $"VectorNF[{this.Dimensions}](";

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
            string txt = $"VectorNF[{this.Dimensions}](";

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
            string txt = $"VectorNF[{this.Dimensions}](";

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
            string txt = $"VectorNF[{this.Dimensions}](";

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
            return value is VectorNF v ? CompareTo(v) : 1;
        }

        public int CompareTo(VectorNF value)
        {
            double l1 = this.Length, l2 = value.Length;

            if (l1 > l2) return 1;

            if (l1 < l2) return -1;

            return 0;
        }
        public bool Equals(VectorNF o)
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
        public static bool operator ==(VectorNF v1, VectorNF v2)
        {
            return v1.Equals(v2);
        }
        public static bool operator !=(VectorNF v1, VectorNF v2)
        {
            return v1.Equals(v2);
        }
        public static VectorNF operator +(VectorNF v, float n)
        {
            VectorNF vec = new VectorNF(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] + n;

            return vec;
        }
        public static VectorNF operator +(VectorNF v1, VectorNF v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot add two vector of different dimensions.");

            VectorNF vec = new VectorNF(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] + v2.Componants[i];

            return vec;
        }
        public static VectorNF operator -(VectorNF v)
        {
            return v * -1.0F;
        }
        public static VectorNF operator -(VectorNF v1, VectorNF v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot substract two vectors of different dimensions.");

            VectorNF vec = new VectorNF(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] - v2.Componants[i];

            return vec;
        }
        public static VectorNF operator *(VectorNF v, float n)
        {
            VectorNF vec = new VectorNF(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] * n;

            return vec;
        }
        public static VectorNF operator *(VectorNF v1, VectorNF v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot multiply two vectors of different dimensions.");

            VectorNF vec = new VectorNF(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] - v2.Componants[i];

            return vec;
        }
        public static VectorNF operator /(VectorNF v, float n)
        {
            VectorNF vec = new VectorNF(v.Dimensions);

            for (int i = 0; i < v.Dimensions; i++)
                vec.Componants[i] = v.Componants[i] / n;

            return vec;
        }
        public static VectorNF operator /(VectorNF v1, VectorNF v2)
        {
            if (v1.Dimensions != v2.Dimensions)
                throw new Exception("Cannot divide two vectors of different dimensions.");

            VectorNF vec = new VectorNF(v1.Dimensions);

            for (int i = 0; i < v1.Dimensions; i++)
                vec.Componants[i] = v1.Componants[i] / v2.Componants[i];

            return vec;
        }
        #endregion
    }
}
