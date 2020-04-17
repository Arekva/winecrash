using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public static class WMath
    {
        public const double DegToRad = Math.PI / 180D;
        public const double RadToDeg = 180D / Math.PI;

        #region CopySign
        internal static Int64 CopySign(Int64 x, Int64 y)
        {
            //if x negative
            if (x < 0L)
            {
                //if y negative
                if (y < 0L)
                {
                    return x;
                }

                //else return positive x
                return x * -1L;
            }

            //if y negative
            if (y < 0L)
            {
                //return negative x
                return x * -1L;
            }

            //else return positive x
            return x;
        }
        public static Int32 CopySign(Int32 x, Int32 y)
        {
            //if x negative
            if (x < 0)
            {
                //if y negative
                if (y < 0)
                {
                    return x;
                }

                //else return positive x
                return x * -1;
            }

            //if y negative
            if (y < 0)
            {
                //return negative x
                return x * -1;
            }

            //else return positive x
            return x;
        }
        public static Int16 CopySign(Int16 x, Int16 y)
        {
            //if x negative
            if (x < 0)
            {
                //if y negative
                if (y < 0)
                {
                    return x;
                }

                //else return positive x
                return (short)(x * -1);
            }

            //if y negative
            if (y < 0)
            {
                //return negative x
                return (short)(x * -1);
            }

            //else return positive x
            return x;
        }
        internal static decimal CopySign(decimal x, decimal y)
        {
            //if x negative
            if (x < 0.0M)
            {
                //if y negative
                if (y < 0.0M)
                {
                    return x;
                }

                //else return positive x
                return x * -1M;
            }

            //if y negative
            if (y < 0.0M)
            {
                //return negative x
                return x * -1M;
            }

            //else return positive x
            return x;
        }
        public static double CopySign(float x, float y)
        {
            //if x negative
            if (x < 0.0F)
            {
                //if y negative
                if (y < 0.0F)
                {
                    return x;
                }

                //else return positive x
                return x * -1.0F;
            }

            //if y negative
            if (y < 0.0F)
            {
                //return negative x
                return x * -1.0F;
            }

            //else return positive x
            return x;
        }
        public static double CopySign(double x, double y)
        {
            //if x negative
            if (x < 0.0D)
            {
                //if y negative
                if (y < 0.0D)
                {
                    return x;
                }

                //else return positive x
                return x * -1.0D;
            }

            //if y negative
            if (y < 0.0D)
            {
                //return negative x
                return x * -1.0D;
            }

            //else return positive x
            return x;
        }
        #endregion
    }
}
