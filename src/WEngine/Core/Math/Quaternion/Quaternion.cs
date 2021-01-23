using Newtonsoft.Json;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine
{
    /// <summary>
    /// Quaternions represent a rotation with an axis.
    /// </summary>
    [Serializable]
    public struct Quaternion
    {
        #region Properties
        [JsonIgnore]
        private double _X;
        /// <summary>
        /// X component of the quaternion. Avoid direct edition.
        /// </summary>
        public double X
        {
            get => _X;
            set => _X = value;
        }
        [JsonIgnore]
        private double _Y;
        /// <summary>
        /// Y component of the quaternion. Avoid direct edition.
        /// </summary>
        public double Y
        {
            get => _Y;
            set => _Y = value;
        }
        [JsonIgnore]
        private double _Z;
        /// <summary>
        /// Z component of the quaternion. Avoid direct edition.
        /// </summary>
        public double Z
        {
            get => _Z;
            set => _Z = value;
        }
        [JsonIgnore]
        private double _W;
        /// <summary>
        /// W component of the quaternion. Avoid direct edition.
        /// </summary>
        public double W
        {
            get => _W;
            set => _W = value;
        }

        /// <summary>
        /// A no-rotation quaternion (0.0, 0.0, 0.0, 1.0)
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return new Quaternion(0.0D, 0.0D, 0.0D, 1.0D);
            }
        }

        /// <summary>
        /// The length of that quaternion.
        /// </summary>
        [JsonIgnore]
        public double Length
        {
            get
            {
                return Math.Sqrt(this.LengthSquared);
            }
        }

        /// <summary>
        /// The squared length of that quaternion. Faster than <see cref="Length"/> but has to be squared rooted.
        /// </summary>
        [JsonIgnore]
        public double LengthSquared
        {
            get
            {
                return new Vector4D(this.X, this.Y, this.Z, this.W).Length;
            }
        }

        /// <summary>
        /// Get or set the euler rotation (degree) of this quaternion.
        /// </summary>
        [JsonIgnore]
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

                angles *= WMath.RadToDeg;

                return angles;
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

        /// <summary>
        /// The direction version of this quaternion.
        /// </summary>
        [JsonIgnore]
        public Quaternion Normalized
        {
            get
            {
                return NormalizeQuaternion(this);
            }
        }
        /// <summary>
        /// The conjugated version of this quaternion.
        /// </summary>
        [JsonIgnore]
        public Quaternion Conjugated
        {
            get
            {
                return ConjugateQuaternion(this);
            }
        }

        /// <summary>
        /// The inverted version of this quaternion.
        /// </summary>
        [JsonIgnore]
        public Quaternion Inverted
        {
            get
            {
                return InverseQuaternion(this);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a quaternion from its basic components. It is better to use <see cref="Quaternion.Quaternion(Vector3D, double)"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        [JsonConstructor]
        public Quaternion(double x, double y, double z, double w)
        {
            this._X = x;
            this._Y = y;
            this._Z = z;
            this._W = w;
        }
        /// <summary>
        /// Create a quaternion given an angle and a rotation axis.
        /// </summary>
        /// <param name="axis">The rotation axis of this quaternion. The axis will be normalized.</param>
        /// <param name="angle">The angle (degree) of the rotation of the quaternion.</param>
        public Quaternion(Vector3D axis, double angle)
        {
            angle *= WMath.DegToRad;

            double halfAngle = angle * 0.5D;
            double s = Math.Sin(halfAngle);
            double c = Math.Cos(halfAngle);

            axis.Normalize();

            this._X = axis.X * s;
            this._Y = axis.Y * s;
            this._Z = axis.Z * s;
            this._W = c;
        }

        /// <summary>
        /// Create a quaternion given euler angles.
        /// </summary>
        /// <param name="angles">The euler angles (degree) of this quaternion.</param>
        public Quaternion(Vector3D angles) : this(angles.X, angles.Y, angles.Z) { }

        /// <summary>
        /// Create a quaternion given euler angles.
        /// </summary>
        /// <param name="xAngle">The X euler angle (degree).</param>
        /// <param name="yAngle">The Y euler angle (degree).</param>
        /// <param name="zAngle">The Z euler angle (degree).</param>
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

            this._X = cy * sp * cr + sy * cp * sr;
            this._Y = sy * cp * cr - cy * sp * sr;
            this._Z = cy * cp * sr - sy * sp * cr;
            this._W = cy * cp * cr + sy * sp * sr;
        }

        /// <summary>
        /// Create a quaternion given a direction and an up vector.
        /// </summary>
        /// <param name="up">The upside vector to project on.</param>
        /// <param name="direction">The direction vector.</param>
        public static Quaternion FromCross(Vector3D up, Vector3D direction)
        {
            up.Normalize();
            direction.Normalize();
            
            double dot = up.X*direction.X + up.Y*direction.Y + up.Z*direction.Z;
            double angle = Math.Acos(WMath.Clamp(dot,-1,1));
            
            return new Quaternion(
                new Vector3D(up.Y*direction.Z - up.Z*direction.Y, up.Z*direction.X - up.X*direction.Z, up.X*direction.Y - up.Y*direction.X), 
                angle*WMath.RadToDeg).Normalized;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get the Axis / Angle of this vector. (Angle defaults to degree)
        /// </summary>
        /// <param name="radian">If the angle should be in radian</param>
        /// <returns>(XYZ) and Angle (W, degrees) of the vector.</returns>
        public Vector4D AxisAngle(bool radian = false)
        {
            Vector4D q = (Vector4D)this;
            if (Math.Abs(q.W) > 1.0f)
            {
                q.Normalize();
            }

            Vector4D result = new Vector4D
            {
                W = 2.0D * Math.Acos(q.W) * (radian ? 1.0D : WMath.RadToDeg) // angle
            };

            double den = Math.Sqrt(1.0D - (q.W * q.W));
            if (den > 0.0001D)
            {
                result.XYZ = q.XYZ / den;
            }
            else
            {
                // This occurs when the angle is zero.
                // Not a problem: just set an arbitrary normalized axis.
                result.XYZ = Vector3D.Up;
            }

            return result;
        }

        /// <summary>
        /// Normalize this quaternion.
        /// </summary>
        /// <returns>This normalized quaternion.</returns>
        public Quaternion Normalize()
        {
            return this = NormalizeQuaternion(this);
        }

        /// <summary>
        /// Conjugate this quaternion.
        /// </summary>
        /// <returns>This conjugated quaternion.</returns>
        public Quaternion Conjugate()
        {
            return this = ConjugateQuaternion(this);
        }
         
        /// <summary>
        /// Inverse this quaternion.
        /// </summary>
        /// <returns>This inverted quaternion.</returns>
        public Quaternion Inverse()
        {
            return this = InverseQuaternion(this);
        }
        
        /// <summary>
        /// Normalize a quaterion.
        /// </summary>
        /// <param name="q">The quaternion to normalize.</param>
        /// <returns>The normalize quaternion.</returns>
        private static Quaternion NormalizeQuaternion(Quaternion q)
        {
            double inv = 1.0D / q.Length;

            q.X *= inv;
            q.Y *= inv;
            q.Z *= inv;
            q.W *= inv;

            return q;
        }
        /// <summary>
        /// Conjugate a quaterion.
        /// </summary>
        /// <param name="q">The quaternion to conjugate.</param>
        /// <returns>The conjugated quaternion.</returns>
        private static Quaternion ConjugateQuaternion(Quaternion q)
        {
            q.X = -q.X;
            q.Y = -q.Y;
            q.Z = -q.Z;

            return q;
        }

        /// <summary>
        /// Invert a quaterion.
        /// </summary>
        /// <param name="q">The quaternion to inverse.</param>
        /// <returns>The inverted quaternion.</returns>
        public static Quaternion InverseQuaternion(Quaternion q)
        {
            double inv = 1.0D / q.Length;

            q.X = -q.X * inv;
            q.Y = -q.Y * inv;
            q.Z = -q.Z * inv;
            q.W *= inv;

            return q;
        }

        /// <summary>
        /// Slerps two quaternion given a time (0..1) to smooth rotations.
        /// </summary>
        /// <param name="q1">The T+0 quaternion.</param>
        /// <param name="q2">The T+1 quaternion.</param>
        /// <param name="t">The time, between 0 and 1.</param>
        /// <returns>The slerped quaternion.</returns>
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

        /// <summary>
        /// Lerps two quaternion given a time (0..1) to smooth rotations.
        /// </summary>
        /// <param name="q1">The T+0 quaternion.</param>
        /// <param name="q2">The T+1 quaternion.</param>
        /// <param name="t">The time, between 0 and 1.</param>
        /// <returns>The lerped quaternion.</returns>
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
        
        public static double Angle(Quaternion a, Quaternion b)
        {
            double dot = Dot(a, b);
            return IsEqualUsingDot(dot) ? 0.0D : Math.Acos(Math.Min(Math.Abs(dot), 1.0D)) * 2.0D * WMath.RadToDeg;
        }
        
        public static double Dot(Quaternion a, Quaternion b)
        {
            return a._X * b._X + a._Y * b._Y + a._Z * b._Z + a._W * b._W;
        }
        
        private const double kEpsilon = 0.000001D;
        
        private static bool IsEqualUsingDot(double dot)
        {
            return dot > 1.0D - kEpsilon;
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(
                a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
                a.W * b.Y + a.Y * b.W + a.Z * b.X - a.X * b.Z,
                a.W * b.Z + a.Z * b.W + a.X * b.Y - a.Y * b.X,
                a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z);
        }

        public static double AngleY(Quaternion a, Quaternion b)
        {
            Vector3D aDir = a * Vector3D.Forward;
            Vector3D bDir = b * Vector3D.Forward;

            double aAngle = Math.Atan2(aDir.X, aDir.Z) * WMath.RadToDeg;
            double bAngle = Math.Atan2(bDir.X, bDir.Z) * WMath.RadToDeg;

            return WMath.DeltaAngle(aAngle, bAngle);
        }

        public override string ToString()
        {
            return $"Quaternion({Math.Round(this.X,4)};{Math.Round(this.Y,4)};{Math.Round(this.Z,4)};{Math.Round(this.W,4)})";
        }


        #endregion
        /// <summary>
        /// Multiply a point by a rotation.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="point">The point to multiply.</param>
        /// <returns>The multiplied point.</returns>
        public static Vector3D operator *(Quaternion rotation, Vector3D point)
        {
            double X = rotation.X * 2.0D;
            double Y = rotation.Y * 2.0D;
            double Z = rotation.Z * 2.0D;
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

        public static explicit operator Vector4D(Quaternion q)
        {
            return new Vector4D(q.X, q.Y, q.Z, q.W);
        }
        public static explicit operator Vector4F(Quaternion q)
        {
            return new Vector4F((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
        }
    }
}
