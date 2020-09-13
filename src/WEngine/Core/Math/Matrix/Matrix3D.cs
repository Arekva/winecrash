using System;
using System.Runtime.InteropServices;

namespace WEngine
{
    /// <summary>
    /// A 3x3 <see cref="double"/> Matrix.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3D : IEquatable<Matrix3D>
    {
        public Vector3D Row0;

        public Vector3D Row1;

        public Vector3D Row2;

        public static readonly Matrix3D Identity = new Matrix3D(Vector3D.Right, Vector3D.Up, Vector3D.Forward);

        public static readonly Matrix3D Zero = new Matrix3D(Vector3D.Zero, Vector3D.Zero, Vector3D.Zero);

        public Matrix3D(Quaternion rotation)
        {
            Vector4D axisAngle = rotation.AxisAngle(true);
            Vector3D axis = axisAngle.XYZ;
            double angle = axisAngle.W;

            axis.Normalize();
            double axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

            // calculate angles
            double cos = Math.Cos(-angle);
            double sin = Math.Sin(-angle);
            double t = 1.0D - cos;

            // do the conversion math once
            double tXX = t * axisX * axisX;
            double tXY = t * axisX * axisY;
            double tXZ = t * axisX * axisZ;
            double tYY = t * axisY * axisY;
            double tYZ = t * axisY * axisZ;
            double tZZ = t * axisZ * axisZ;

            double sinX = sin * axisX;
            double sinY = sin * axisY;
            double sinZ = sin * axisZ;

            Row0 = new Vector3D();
            Row1 = new Vector3D();
            Row2 = new Vector3D();

            this.Row0.X = tXX + cos;
            this.Row0.Y = tXY - sinZ;
            this.Row0.Z = tXZ + sinY;
            this.Row1.X = tXY + sinZ;
            this.Row1.Y = tYY + cos;
            this.Row1.Z = tYZ - sinX;
            this.Row2.X = tXZ - sinY;
            this.Row2.Y = tYZ + sinX;
            this.Row2.Z = tZZ + cos;
        }
        public Matrix3D(Vector3D scale)
        {
            this = Identity;
            this.Row0.X = scale.X;
            this.Row1.Y = scale.Y;
            this.Row2.Z = scale.Z;
        }
        public Matrix3D(Vector3D row0, Vector3D row1, Vector3D row2)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
        }
        
        public Matrix3D
        (
            double m00, double m01, double m02,
            double m10, double m11, double m12,
            double m20, double m21, double m22
        )
        {
            Row0 = new Vector3D(m00, m01, m02);
            Row1 = new Vector3D(m10, m11, m12);
            Row2 = new Vector3D(m20, m21, m22);
        }

        public Matrix3D(Matrix4D matrix)
        {
            Row0 = matrix.Row0.XYZ;
            Row1 = matrix.Row1.XYZ;
            Row2 = matrix.Row2.XYZ;
        }

        public double Determinant
        {
            get
            {
                double m11 = Row0.X;
                double m12 = Row0.Y;
                double m13 = Row0.Z;
                double m21 = Row1.X;
                double m22 = Row1.Y;
                double m23 = Row1.Z;
                double m31 = Row2.X;
                double m32 = Row2.Y;
                double m33 = Row2.Z;

                return (m11 * m22 * m33) + (m12 * m23 * m31) + (m13 * m21 * m32)
                       - (m13 * m22 * m31) - (m11 * m23 * m32) - (m12 * m21 * m33);
            }
        }

        public Vector3D Column0 => new Vector3D(Row0.X, Row1.X, Row2.X);
        public Vector3D Column1 => new Vector3D(Row0.Y, Row1.Y, Row2.Y);
        public Vector3D Column2 => new Vector3D(Row0.Z, Row1.Z, Row2.Z);

        public double M11
        {
            get => Row0.X;
            set => Row0.X = value;
        }

        public double M12
        {
            get => Row0.Y;
            set => Row0.Y = value;
        }

        public double M13
        {
            get => Row0.Z;
            set => Row0.Z = value;
        }

        public double M21
        {
            get => Row1.X;
            set => Row1.X = value;
        }

        public double M22
        {
            get => Row1.Y;
            set => Row1.Y = value;
        }

        public double M23
        {
            get => Row1.Z;
            set => Row1.Z = value;
        }

        public double M31
        {
            get => Row2.X;
            set => Row2.X = value;
        }

        public double M32
        {
            get => Row2.Y;
            set => Row2.Y = value;
        }

        public double M33
        {
            get => Row2.Z;
            set => Row2.Z = value;
        }

        public Vector3D Diagonal
        {
            get => new Vector3D(Row0.X, Row1.Y, Row2.Z);
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
            }
        }

        public double Trace => Row0.X + Row1.Y + Row2.Z;

        public double this[int rowIndex, int columnIndex]
        {
            get
            {
                switch (rowIndex)
                {
                    case 0:
                        return Row0[columnIndex];

                    case 1:
                        return Row1[columnIndex];

                    case 2:
                        return Row2[columnIndex];

                    default:
                        throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " +
                                                           columnIndex + ")");
                }
            }

            set
            {
                switch (rowIndex)
                {
                    case 0:
                        Row0[columnIndex] = value;
                        break;

                    case 1:
                        Row1[columnIndex] = value;
                        break;

                    case 2:
                        Row2[columnIndex] = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " +
                                                       columnIndex + ")");
                }
            }
        }

        public Matrix3D Inverted
        {
            get
            {
                Matrix3D m = this;
                if (m.Determinant != 0)
                {
                    m.Invert();
                }

                return m;
            }
        }
        public Matrix3D Normalized
        {
            get
            {
                Matrix3D m = this;
                m.Normalize();
                return m;
            }
        }

        public void Invert()
        {
            int[] colIdx = { 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1 };

            double[,] inverse =
            {
                { this.Row0.X, this.Row0.Y, this.Row0.Z },
                { this.Row1.X, this.Row1.Y, this.Row1.Z },
                { this.Row2.X, this.Row2.Y, this.Row2.Z }
            };

            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 3; i++)
            {
                double maxPivot = 0.0D;
                for (int j = 0; j < 3; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                double absVal = Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return;
                            }
                        }
                    }
                }

                ++pivotIdx[icol];

                if (irow != icol)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        double f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                double pivot = inverse[icol, icol];

                if (pivot == 0.0D)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }

                double oneOverPivot = 1.0D / pivot;
                inverse[icol, icol] = 1.0D;
                for (int k = 0; k < 3; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                for (int j = 0; j < 3; ++j)
                {
                    if (icol != j)
                    {
                        double f = inverse[j, icol];
                        inverse[j, icol] = 0.0D;
                        for (int k = 0; k < 3; ++k)
                        {
                            inverse[j, k] -= inverse[icol, k] * f;
                        }
                    }
                }
            }

            for (int j = 2; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 3; ++k)
                {
                    double f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            this.Row0.X = inverse[0, 0];
            this.Row0.Y = inverse[0, 1];
            this.Row0.Z = inverse[0, 2];
            this.Row1.X = inverse[1, 0];
            this.Row1.Y = inverse[1, 1];
            this.Row1.Z = inverse[1, 2];
            this.Row2.X = inverse[2, 0];
            this.Row2.Y = inverse[2, 1];
            this.Row2.Z = inverse[2, 2];
        }

        public void Normalize()
        {
            double determinant = Determinant;
            Row0 /= determinant;
            Row1 /= determinant;
            Row2 /= determinant;
        }

        public Matrix3D ClearedScale
        {
            get
            {
                Matrix3D m = this;
                m.Row0 = m.Row0.Normalized;
                m.Row1 = m.Row1.Normalized;
                m.Row2 = m.Row2.Normalized;
                return m;
            }
        }
        public Matrix3D ClearedRotation
        {
            get
            {
                Matrix3D m = this;
                m.Row0 = new Vector3D(m.Row0.Length, 0, 0);
                m.Row1 = new Vector3D(0, m.Row1.Length, 0);
                m.Row2 = new Vector3D(0, 0, m.Row2.Length);
                return m;
            }
        }

        public Vector3D ExtractedScale
        {
            get
            {
                return new Vector3D(Row0.Length, Row1.Length, Row2.Length);
            }
        }
        public Quaternion ExtractedRotation
        {
            get
            {
                Vector3D row0 = Row0.Normalized;
                Vector3D row1 = Row1.Normalized;
                Vector3D row2 = Row2.Normalized;

                // code below adapted from Blender
                Quaternion q = new Quaternion();
                double trace = 0.25D * (row0[0] + row1[1] + row2[2] + 1.0D);

                if (trace > 0.0D)
                {
                    double sq = Math.Sqrt(trace);

                    q.W = sq;
                    sq = 1.0D / (4.0D * sq);
                    q.X = (row1[2] - row2[1]) * sq;
                    q.Y = (row2[0] - row0[2]) * sq;
                    q.Z = (row0[1] - row1[0]) * sq;
                }
                else if (row0[0] > row1[1] && row0[0] > row2[2])
                {
                    double sq = 2.0D * Math.Sqrt(1.0D + row0[0] - row1[1] - row2[2]);

                    q.X = 0.25D * sq;
                    sq = 1.0D / sq;
                    q.W = (row2[1] - row1[2]) * sq;
                    q.Y = (row1[0] + row0[1]) * sq;
                    q.Z = (row2[0] + row0[2]) * sq;
                }
                else if (row1[1] > row2[2])
                {
                    double sq = 2.0D * Math.Sqrt(1.0D + row1[1] - row0[0] - row2[2]);

                    q.Y = 0.25D * sq;
                    sq = 1.0 / sq;
                    q.W = (row2[0] - row0[2]) * sq;
                    q.X = (row1[0] + row0[1]) * sq;
                    q.Z = (row2[1] + row1[2]) * sq;
                }
                else
                {
                    double sq = 2.0D * Math.Sqrt(1.0D + row2[2] - row0[0] - row1[1]);

                    q.Z = 0.25D * sq;
                    sq = 1.0D / sq;
                    q.W = (row1[0] - row0[1]) * sq;
                    q.X = (row2[0] + row0[2]) * sq;
                    q.Y = (row2[1] + row1[2]) * sq;
                }

                q.Normalize();
                return q;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Matrix3D mat ? Equals(mat) : false;
        }

        public bool Equals(Matrix3D other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Row0.GetHashCode();
                hashCode = (hashCode * 397) ^ Row1.GetHashCode();
                hashCode = (hashCode * 397) ^ Row2.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Row0}\n{Row1}\n{Row2}";
        }


        public static bool operator == (Matrix3D left, Matrix3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3D left, Matrix3D right)
        {
            return !left.Equals(right);
        }


        public static Matrix3D operator * (Matrix3D left, Matrix3D right)
        {
            double leftM11 = left.Row0.X;
            double leftM12 = left.Row0.Y;
            double leftM13 = left.Row0.Z;
            double leftM21 = left.Row1.X;
            double leftM22 = left.Row1.Y;
            double leftM23 = left.Row1.Z;
            double leftM31 = left.Row2.X;
            double leftM32 = left.Row2.Y;
            double leftM33 = left.Row2.Z;
            double rightM11 = right.Row0.X;
            double rightM12 = right.Row0.Y;
            double rightM13 = right.Row0.Z;
            double rightM21 = right.Row1.X;
            double rightM22 = right.Row1.Y;
            double rightM23 = right.Row1.Z;
            double rightM31 = right.Row2.X;
            double rightM32 = right.Row2.Y;
            double rightM33 = right.Row2.Z;

            Matrix3D result = new Matrix3D();

            result.Row0.X = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31);
            result.Row0.Y = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32);
            result.Row0.Z = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33);
            result.Row1.X = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31);
            result.Row1.Y = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32);
            result.Row1.Z = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33);
            result.Row2.X = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31);
            result.Row2.Y = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32);
            result.Row2.Z = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33);

            return result;
        }

        public static Matrix3D operator + (Matrix3D left, Matrix3D right)
        {
            return new Matrix3D(left.Row0 + right.Row0, left.Row1 + right.Row1, left.Row2 + right.Row2);
        }

    }
}
