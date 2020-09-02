using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using OpenCL.Net;

namespace Winecrash.Engine
{
    /// <summary>
    /// todo: finish Matrix4D
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4D// : IEquatable<Matrix4D>
    {
        public Vector4D Row0;
        public Vector4D Row1;
        public Vector4D Row2;
        public Vector4D Row3;

        public static readonly Matrix4D Identity = new Matrix4D(Vector4D.Right, Vector4D.Up, Vector4D.Forward, Vector4D.Ana);

        public static readonly Matrix4D Zero = new Matrix4D(Vector4D.Zero, Vector4D.Zero, Vector4D.Zero, Vector4D.Zero);

        public Matrix4D(Vector4D row0, Vector4D row1, Vector4D row2, Vector4D row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public Matrix4D
        (
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33
        )
        {
            Row0 = new Vector4D(m00, m01, m02, m03);
            Row1 = new Vector4D(m10, m11, m12, m13);
            Row2 = new Vector4D(m20, m21, m22, m23);
            Row3 = new Vector4D(m30, m31, m32, m33);
        }

        public Matrix4D(Matrix3D topLeft)
        {
            Row0 = new Vector4D(topLeft.Row0, 0.0D);
            Row1 = new Vector4D(topLeft.Row1, 0.0D);
            Row2 = new Vector4D(topLeft.Row2, 0.0D);
            Row3 = new Vector4D(Vector3D.Zero, 1.0D);
        }

        public double Determinant
        {
            get
            {
                double m11 = Row0.X;
                double m12 = Row0.Y;
                double m13 = Row0.Z;
                double m14 = Row0.W;
                double m21 = Row1.X;
                double m22 = Row1.Y;
                double m23 = Row1.Z;
                double m24 = Row1.W;
                double m31 = Row2.X;
                double m32 = Row2.Y;
                double m33 = Row2.Z;
                double m34 = Row2.W;
                double m41 = Row3.X;
                double m42 = Row3.Y;
                double m43 = Row3.Z;
                double m44 = Row3.W;

                return
                    (m11 * m22 * m33 * m44) - (m11 * m22 * m34 * m43) + (m11 * m23 * m34 * m42) - (m11 * m23 * m32 * m44)
                    + (m11 * m24 * m32 * m43) - (m11 * m24 * m33 * m42) - (m12 * m23 * m34 * m41) + (m12 * m23 * m31 * m44)
                    - (m12 * m24 * m31 * m43) + (m12 * m24 * m33 * m41) - (m12 * m21 * m33 * m44) + (m12 * m21 * m34 * m43)
                                                                                            + (m13 * m24 * m31 * m42) -
                    (m13 * m24 * m32 * m41) + (m13 * m21 * m32 * m44) - (m13 * m21 * m34 * m42)
                    + (m13 * m22 * m34 * m41) - (m13 * m22 * m31 * m44) - (m14 * m21 * m32 * m43) + (m14 * m21 * m33 * m42)
                    - (m14 * m22 * m33 * m41) + (m14 * m22 * m31 * m43) - (m14 * m23 * m31 * m42) + (m14 * m23 * m32 * m41);
            }
        }

        public Vector4D Column0
        {
            get => new Vector4D(Row0.X, Row1.X, Row2.X, Row3.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
                Row2.X = value.Z;
                Row3.X = value.W;
            }
        }

        public Vector4D Column1
        {
            get => new Vector4D(Row0.Y, Row1.Y, Row2.Y, Row3.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
                Row2.Y = value.Z;
                Row3.Y = value.W;
            }
        }

        public Vector4D Column2
        {
            get => new Vector4D(Row0.Z, Row1.Z, Row2.Z, Row3.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
                Row2.Z = value.Z;
                Row3.Z = value.W;
            }
        }

        public Vector4D Column3
        {
            get => new Vector4D(Row0.W, Row1.W, Row2.W, Row3.W);
            set
            {
                Row0.W = value.X;
                Row1.W = value.Y;
                Row2.W = value.Z;
                Row3.W = value.W;
            }
        }

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

        public double M14
        {
            get => Row0.W;
            set => Row0.W = value;
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

        public double M24
        {
            get => Row1.W;
            set => Row1.W = value;
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
        
        public double M34
        {
            get => Row2.W;
            set => Row2.W = value;
        }

        public double M41
        {
            get => Row3.X;
            set => Row3.X = value;
        }

        public double M42
        {
            get => Row3.Y;
            set => Row3.Y = value;
        }
        
        public double M43
        {
            get => Row3.Z;
            set => Row3.Z = value;
        }
        
        public double M44
        {
            get => Row3.W;
            set => Row3.W = value;
        }

        public Vector4D Diagonal
        {
            get => new Vector4D(Row0.X, Row1.Y, Row2.Z, Row3.W);
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
                Row3.W = value.W;
            }
        }

        public double Trace => Row0.X + Row1.Y + Row2.Z + Row3.W;

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

                    case 3:
                        return Row3[columnIndex];

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

                    case 3:
                        Row3[columnIndex] = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " +
                                               columnIndex + ")");
                }
            }
        }

        public Matrix4D Inverted
        {
            get
            {
                Matrix4D m = this;
                if (m.Determinant != 0)
                {
                    m.Invert();
                }

                return m;
            }
        }

        public void Invert()
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            double[,] inverse =
            {
                { this.Row0.X, this.Row0.Y, this.Row0.Z, this.Row0.W },
                { this.Row1.X, this.Row1.Y, this.Row1.Z, this.Row1.W },
                { this.Row2.X, this.Row2.Y, this.Row2.Z, this.Row2.W },
                { this.Row3.X, this.Row3.Y, this.Row3.Z, this.Row3.W }
            };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                double maxPivot = 0.0D;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
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

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        double f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                double pivot = inverse[icol, icol];

                // check for singular matrix
                if (pivot == 0.0D)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }

                // Scale row so it has a unit diagonal
                double oneOverPivot = 1.0D / pivot;
                inverse[icol, icol] = 1.0D;
                for (int k = 0; k < 4; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        double f = inverse[j, icol];
                        inverse[j, icol] = 0.0D;
                        for (int k = 0; k < 4; ++k)
                        {
                            inverse[j, k] -= inverse[icol, k] * f;
                        }
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    double f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            this.Row0.X = inverse[0, 0];
            this.Row0.Y = inverse[0, 1];
            this.Row0.Z = inverse[0, 2];
            this.Row0.W = inverse[0, 3];
            this.Row1.X = inverse[1, 0];
            this.Row1.Y = inverse[1, 1];
            this.Row1.Z = inverse[1, 2];
            this.Row1.W = inverse[1, 3];
            this.Row2.X = inverse[2, 0];
            this.Row2.Y = inverse[2, 1];
            this.Row2.Z = inverse[2, 2];
            this.Row2.W = inverse[2, 3];
            this.Row3.X = inverse[3, 0];
            this.Row3.Y = inverse[3, 1];
            this.Row3.Z = inverse[3, 2];
            this.Row3.W = inverse[3, 3];
        }


        public Matrix4D Normalized
        {
            get
            {
                Matrix4D m = this;
                m.Normalize();
                return m;
            }
        }

        public void Normalize()
        {
            double determinant = Determinant;
            Row0 /= determinant;
            Row1 /= determinant;
            Row2 /= determinant;
            Row3 /= determinant;
        }
    }
}
