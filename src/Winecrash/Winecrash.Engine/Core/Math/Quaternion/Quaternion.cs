using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public struct Quaternion
    {
        #region Properties
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public static Quaternion Identity
        {
            get
            {
                return new Quaternion(0.0D, 0.0D, 0.0D, 1.0D);
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(this.LengthSquared);
            }
        }

        public double LengthSquared
        {
            get
            {
                return new Vector4D(this.X, this.Y, this.Z, this.W).Length;
            }
        }

        public Vector3D Euler
        {
            get
            {
                Quaternion q = this;
                Vector3D angles = new Vector3D(0.0D);

                // roll (X-axis rotation)
                double sinr_cosp = 2.0D * (q.W * q.X + q.Y * q.Z);
                double cosr_cosp = 1.0D - 2.0D * (q.X * q.X + q.Y * q.Y);
                angles.X = Math.Atan2(sinr_cosp, cosr_cosp);

                // pitch (Y-axis rotation)
                double sinp = 2.0D * (q.W * q.Y - q.Z * q.X);
                if (Math.Abs(sinp) >= 1.0D)
                    angles.Y = WMath.CopySign(Math.PI / 2.0D, sinp); // use 90 degrees if out of range
                else
                    angles.Y = Math.Asin(sinp);

                // yaw (Z-axis rotation)
                double siny_cosp = 2.0D * (q.W * q.Z + q.X * q.Y);
                double cosy_cosp = 1.0D - 2.0D * (q.Y * q.Y + q.Z * q.Z);
                angles.Z = Math.Atan2(siny_cosp, cosy_cosp);

                return angles * WMath.RadToDeg;
            }
            set
            {
                this = new Quaternion(value);
            }
        }

        /*public Vector3D Direction
        {
            get
            {
                Vector3D angles = this.Euler * WMath.DegToRad;

                Vector3D dir = new Vector3D();

                double xCos = Math.Sin(angles.X);
                double xSin = Math.Sin(angles.X);
                double yCos = Math.Cos(angles.Y);
                double ySin = Math.Sin(angles.Y);
                double zCos = Math.Cos(angles.Z);
                double zSin = Math.Sin(angles.Z);

                // x: yaw
                // y: pitch
                // z: roll

                dir.X = -xCos * ySin * zSin - xSin * zCos;
                dir.Y = -xSin * ySin * zSin + xCos * zCos;
                dir.Z = yCos * zSin;

                return dir;//.Normalized;
            }
        }*/

        public Quaternion Normalized
        {
            get
            {
                return NormalizeQuaternion(this);
            }
        }

        public Quaternion Conjugated
        {
            get
            {
                return ConjugateQuaternion(this);
            }
        }

        public Quaternion Inverted
        {
            get
            {
                return InverseQuaternion(this);
            }
        }
        #endregion

        #region Constructors
        public Quaternion(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        public Quaternion(Vector3D axis, double angle)
        {
            angle *= WMath.DegToRad;

            double halfAngle = angle * 0.5D;
            double s = Math.Sin(halfAngle);
            double c = Math.Cos(halfAngle);

            this.X = axis.X * s;
            this.Y = axis.Y * s;
            this.Z = axis.Z * s;
            this.W = c;
        }

        public Quaternion(Vector3D angles)
        {
            angles *= WMath.DegToRad;

            double sr, cr, sp, cp, sy, cy;

            double halfRoll = angles.Z * 0.5D;
            sr = Math.Sin(halfRoll);
            cr = Math.Cos(halfRoll);

            double halfPitch = angles.X * 0.5D;
            sp = Math.Sin(halfPitch);
            cp = Math.Cos(halfPitch);

            double halfYaw = angles.Y * 0.5D;
            sy = Math.Sin(halfYaw);
            cy = Math.Cos(halfYaw);

            this.X = cy * sp * cr + sy * cp * sr;
            this.Y = sy * cp * cr - cy * sp * sr;
            this.Z = cy * cp * sr - sy * sp * cr;
            this.W = cy * cp * cr + sy * sp * sr;
        }

        public Quaternion(double xAngle, double yAngle, double zAngle)
        {
            xAngle *= WMath.DegToRad;
            yAngle *= WMath.DegToRad;
            zAngle *= WMath.DegToRad;

            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            double sr, cr, sp, cp, sy, cy;

            double halfRoll = zAngle * 0.5D;
            sr = Math.Sin(halfRoll);
            cr = Math.Cos(halfRoll);

            double halfPitch = xAngle * 0.5D;
            sp = Math.Sin(halfPitch);
            cp = Math.Cos(halfPitch);

            double halfYaw = yAngle * 0.5D;
            sy = Math.Sin(halfYaw);
            cy = Math.Cos(halfYaw);

            this.X = cy * sp * cr + sy * cp * sr;
            this.Y = sy * cp * cr - cy * sp * sr;
            this.Z = cy * cp * sr - sy * sp * cr;
            this.W = cy * cp * cr + sy * sp * sr;
        }
        #endregion

        #region Methods
        public Quaternion Normalize()
        {
            return this = NormalizeQuaternion(this);
        }

        public Quaternion Conjugate()
        {
            return this = ConjugateQuaternion(this);
        }

        public Quaternion Inverse()
        {
            return this = InverseQuaternion(this);
        }
        
        private static Quaternion NormalizeQuaternion(Quaternion q)
        {
            double inv = 1.0D / q.Length;

            q.X *= inv;
            q.Y *= inv;
            q.Z *= inv;
            q.W *= inv;

            return q;
        }

        private static Quaternion ConjugateQuaternion(Quaternion q)
        {
            q.X = -q.X;
            q.Y = -q.Y;
            q.Z = -q.Z;

            return q;
        }

        public static Quaternion InverseQuaternion(Quaternion q)
        {
            double inv = 1.0D / q.Length;

            q.X = -q.X * inv;
            q.Y = -q.Y * inv;
            q.Z = -q.Z * inv;
            q.W *= inv;

            return q;
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, double t)
        {
            double cosOmega = q1.X * q2.X + q1.Y * q2.Y +
                             q1.Z * q2.Z + q1.W * q2.W;

            bool flip = false;

            if (cosOmega < 0.0D)
            {
                flip = true;
                cosOmega = -cosOmega;
            }

            double s1, s2;

            if (cosOmega > (1.0D - Double.Epsilon))
            {
                // Too close, do straight linear interpolation.
                s1 = 1.0D - t;
                s2 = (flip) ? -t : t;
            }
            else
            {
                double omega = Math.Acos(cosOmega);
                double invSinOmega = 1.0D / Math.Sin(omega);

                s1 = Math.Sin((1.0D - t) * omega) * invSinOmega;
                s2 = (flip)
                    ? -Math.Sin(t * omega) * invSinOmega
                    : Math.Sin(t * omega) * invSinOmega;
            }

            Quaternion ans = new Quaternion
            {
                X = s1 * q1.X + s2 * q2.X,
                Y = s1 * q1.Y + s2 * q2.Y,
                Z = s1 * q1.Z + s2 * q2.Z,
                W = s1 * q1.W + s2 * q2.W
            };

            return ans;
        }

        public static Quaternion Lerp(Quaternion q1, Quaternion q2, double t)
        {
            double t1 = 1.0D - t;

            Quaternion r = new Quaternion();

            double dot = q1.X * q2.X + q1.Y * q2.Y +
                        q1.Z * q2.Z + q1.W * q2.W;

            if (dot >= 0.0D)
            {
                r.X = t1 * q1.X + t * q2.X;
                r.Y = t1 * q1.Y + t * q2.Y;
                r.Z = t1 * q1.Z + t * q2.Z;
                r.W = t1 * q1.W + t * q2.W;
            }
            else
            {
                r.X = t1 * q1.X - t * q2.X;
                r.Y = t1 * q1.Y - t * q2.Y;
                r.Z = t1 * q1.Z - t * q2.Z;
                r.W = t1 * q1.W - t * q2.W;
            }

            // Normalize it.
            double ls = r.Length;
            double invNorm = 1.0D / Math.Sqrt(ls);

            r.X *= invNorm;
            r.Y *= invNorm;
            r.Z *= invNorm;
            r.W *= invNorm;

            return r;
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        public override string ToString()
        {
            return $"Quaternion({Math.Round(this.X,4)};{Math.Round(this.Y,4)};{Math.Round(this.Z,4)};{Math.Round(this.W,4)})";
        }


        #endregion

        public static Vector3D operator *(Quaternion rotation, Vector3D point)
        {
            double X = rotation.X * 2.0D;
            double Y = rotation.Y * 2.0D;
            double Z = rotation.Z * 2F;
            double XX = rotation.X * X;
            double YY = rotation.Y * Y;
            double ZZ = rotation.Z * Z;
            double XY = rotation.X * Y;
            double XZ = rotation.X * Z;
            double YZ = rotation.Y * Z;
            double WX = rotation.W * X;
            double WY = rotation.W * Y;
            double WZ = rotation.W * Z;

            Vector3D res = Vector3D.Zero;
            res.X = (1.0D - (YY + ZZ)) * point.X + (XY - WZ) * point.Y + (XZ + WY) * point.Z;
            res.Y = (XY + WZ) * point.X + (1.0D - (XX + ZZ)) * point.Y + (YZ - WX) * point.Z;
            res.Z = (XZ - WY) * point.X + (YZ + WX) * point.Y + (1.0D - (XX + YY)) * point.Z;
            return res;
        }

        public static Vector3F operator *(Quaternion rotation, Vector3F point)
        {
            return rotation * (Vector3D)point;
        }
    }
}
