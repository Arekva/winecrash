using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using OpenCL.Net;
using OpenTK;

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



        /// <summary>
        /// Create perspective matrix
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspect"></param>
        /// <param name="depthNear"></param>
        /// <param name="depthFar"></param>
        public Matrix4D(double fovy, double aspect, double depthNear, double depthFar)
        {
            if (fovy <= 0.0D || fovy > Math.PI)
            {
                throw new ArgumentOutOfRangeException(nameof(fovy));
            }

            if (aspect <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(aspect));
            }

            if (depthNear <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            if (depthFar <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthFar));
            }

            double maxY = depthNear * Math.Tan(0.5D * fovy);
            double minY = -maxY;
            double minX = minY * aspect;
            double maxX = maxY * aspect;

            this = new Matrix4D(minX, maxX, minY, maxY, depthNear, depthFar);

        }

        /// <summary>
        /// Create off-center perspetive matrix
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="depthNear"></param>
        /// <param name="depthFar"></param>
        public Matrix4D(double left, double right, double bottom, double top, double depthNear, double depthFar)
        {
            if (depthNear <= 0.0D)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            if (depthFar <= 0.0D)
            {
                throw new ArgumentOutOfRangeException(nameof(depthFar));
            }

            if (depthNear >= depthFar)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            double x = 2.0D * depthNear / (right - left);
            double y = 2.0D * depthNear / (top - bottom);
            double a = (right + left) / (right - left);
            double b = (top + bottom) / (top - bottom);
            double c = -(depthFar + depthNear) / (depthFar - depthNear);
            double d = -(2.0D * depthFar * depthNear) / (depthFar - depthNear);

            this.Row0 = new Vector4D(x, 0.0D, 0.0D, 0.0D);
            this.Row1 = new Vector4D(0.0D, y, 0.0D, 0.0D);
            this.Row2 = new Vector4D(a, b, c, -1.0D);
            this.Row3 = new Vector4D(0.0D, 0.0D, d, 0.0D);
        }

        /// <summary>
        /// Create a matrix when top left is existing Matrix3D
        /// </summary>
        /// <param name="topLeft"></param>
        public Matrix4D(Matrix3D topLeft)
        {
            Row0 = new Vector4D(topLeft.Row0, 0.0D);
            Row1 = new Vector4D(topLeft.Row1, 0.0D);
            Row2 = new Vector4D(topLeft.Row2, 0.0D);
            Row3 = new Vector4D(Vector3D.Zero, 1.0D);
        }

        /// <summary>
        /// Create transalation / scale
        /// Set scale to true to make a scale matrix.
        /// If false, it's a translation matrix
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="scale"></param>
        public Matrix4D(Vector3D vec, bool scale = false)
        {
            if (scale)
            {
                this = Matrix4D.Identity;

                this.Row0.X = vec.X;
                this.Row1.Y = vec.Y;
                this.Row2.Z = vec.Z;
            }

            else
            {
                this = Matrix4D.Identity;

                this.Row3.X = vec.X;
                this.Row3.Y = vec.Y;
                this.Row3.Z = vec.Z;
            }
        }
        /// <summary>
        /// Create a rotation matrix
        /// </summary>
        /// <param name="rot"></param>
        public Matrix4D(Quaternion rot)
        {
            Vector4D axisAngle = rot.AxisAngle(true);

            Vector3D axis = axisAngle.XYZ;
            double angle = axisAngle.W;

            // normalize and create a local copy of the vector.
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

            this.Row0 = new Vector4D(tXX + cos, tXY - sinZ, tXZ + sinY, 0.0D);
            this.Row1 = new Vector4D(tXY + sinZ, tYY + cos, tYZ - sinX, 0.0D);
            this.Row2 = new Vector4D(tXZ - sinY, tYZ + sinX, tZZ + cos, 0.0D);
            this.Row3 = Vector4D.Ana;
        }
        /// <summary>
        /// Create LookAt matrix
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="target"></param>
        /// <param name="up"></param>
        public Matrix4D(Vector3D eye, Vector3D target, Vector3D up)
        {
            Vector3D z = (eye - target).Normalize();
            Vector3D x = Vector3D.Cross(up, z).Normalize();
            Vector3D y = Vector3D.Cross(z, x).Normalize();

            this.Row0 = new Vector4D(x.X, y.X, z.X, 0.0D);
            this.Row1 = new Vector4D(x.Y, y.Y, z.Y, 0.0D);
            this.Row2 = new Vector4D(x.Z, y.Z, z.Z, 0.0D);
            this.Row3 = new Vector4D(
                -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z)),
                -((y.X * eye.X) + (y.Y * eye.Y) + (y.Z * eye.Z)),
                -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z)),
                1.0D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depthNear"></param>
        /// <param name="depthFar"></param>
        public static Matrix4D Orthographic(double width, double height, double depthNear, double depthFar)
        {
            return CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, depthNear, depthFar);
        }

        public static Matrix4D CreateOrthographicOffCenter(double left, double right, double bottom, double top, double depthNear, double depthFar)
        {
            // ok
            Matrix4D result = Identity;

            double invRL = 1.0D / (right - left);
            double invTB = 1.0D / (top - bottom);
            double invFN = 1.0D / (depthFar - depthNear);

            result.Row0.X = 2.0D * invRL;
            result.Row1.Y = 2.0D * invTB;
            result.Row2.Z = -2.0D * invFN;

            result.Row3.X = -(right + left) * invRL;
            result.Row3.Y = -(top + bottom) * invTB;
            result.Row3.Z = -(depthFar + depthNear) * invFN;

            return result;
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

        public static void LookAtRef(Vector3D eye, Vector3D target, Vector3D up, out Matrix4D result)
        {
            Vector3D z = (eye - target).Normalize();
            Vector3D x = Vector3D.Cross(up, z).Normalize();
            Vector3D y = Vector3D.Cross(z, x).Normalize();

            result.Row0 = new Vector4D(x.X, y.X, z.X, 0.0D);
            result.Row1 = new Vector4D(x.Y, y.Y, z.Y, 0.0D);
            result.Row2 = new Vector4D(x.Z, y.Z, z.Z, 0.0D);
            result.Row3 = new Vector4D(
                -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z)),
                -((y.X * eye.X) + (y.Y * eye.Y) + (y.Z * eye.Z)),
                -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z)),
                1.0D);
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

        public static explicit operator Matrix4(Matrix4D m)
        {
            return new Matrix4((Vector4F)m.Row0, (Vector4F)m.Row1, (Vector4F)m.Row2, (Vector4F)m.Row3);
        }

        public static Matrix4D operator *(Matrix4D left, Matrix4D right)
        {
            Mult(in left, in right, out Matrix4D res);
            return res;
        }

        public static void Mult(in Matrix4D left, in Matrix4D right, out Matrix4D result)
        //public static Matrix4D operator * (Matrix4D left, Matrix4D right)
        {
            double leftM11 = left.Row0.X;
            double leftM12 = left.Row0.Y;
            double leftM13 = left.Row0.Z;
            double leftM14 = left.Row0.W;
            double leftM21 = left.Row1.X;
            double leftM22 = left.Row1.Y;
            double leftM23 = left.Row1.Z;
            double leftM24 = left.Row1.W;
            double leftM31 = left.Row2.X;
            double leftM32 = left.Row2.Y;
            double leftM33 = left.Row2.Z;
            double leftM34 = left.Row2.W;
            double leftM41 = left.Row3.X;
            double leftM42 = left.Row3.Y;
            double leftM43 = left.Row3.Z;
            double leftM44 = left.Row3.W;
            double rightM11 = right.Row0.X;
            double rightM12 = right.Row0.Y;
            double rightM13 = right.Row0.Z;
            double rightM14 = right.Row0.W;
            double rightM21 = right.Row1.X;
            double rightM22 = right.Row1.Y;
            double rightM23 = right.Row1.Z;
            double rightM24 = right.Row1.W;
            double rightM31 = right.Row2.X;
            double rightM32 = right.Row2.Y;
            double rightM33 = right.Row2.Z;
            double rightM34 = right.Row2.W;
            double rightM41 = right.Row3.X;
            double rightM42 = right.Row3.Y;
            double rightM43 = right.Row3.Z;
            double rightM44 = right.Row3.W;

            result.Row0 = new Vector4D( (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31) + (leftM14 * rightM41),
                                        (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32) + (leftM14 * rightM42),
                                        (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33) + (leftM14 * rightM43),
                                        (leftM11 * rightM14) + (leftM12 * rightM24) + (leftM13 * rightM34) + (leftM14 * rightM44));

            result.Row1 = new Vector4D( (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31) + (leftM24 * rightM41),
                                        (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32) + (leftM24 * rightM42),
                                        (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33) + (leftM24 * rightM43),
                                        (leftM21 * rightM14) + (leftM22 * rightM24) + (leftM23 * rightM34) + (leftM24 * rightM44));


            result.Row2 = new Vector4D( (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31) + (leftM34 * rightM41),
                                        (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32) + (leftM34 * rightM42),
                                        (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33) + (leftM34 * rightM43),
                                        (leftM31 * rightM14) + (leftM32 * rightM24) + (leftM33 * rightM34) + (leftM34 * rightM44));

            result.Row3 = new Vector4D( (leftM41 * rightM11) + (leftM42 * rightM21) + (leftM43 * rightM31) + (leftM44 * rightM41),
                                        (leftM41 * rightM12) + (leftM42 * rightM22) + (leftM43 * rightM32) + (leftM44 * rightM42),
                                        (leftM41 * rightM13) + (leftM42 * rightM23) + (leftM43 * rightM33) + (leftM44 * rightM43),
                                        (leftM41 * rightM14) + (leftM42 * rightM24) + (leftM43 * rightM34) + (leftM44 * rightM44));
        }
    }
}
