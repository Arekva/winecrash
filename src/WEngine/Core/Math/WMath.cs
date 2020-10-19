using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine
{
    /// <summary>
    /// The math utilities.
    /// </summary>
    public static class WMath
    {
        /// <summary>
        /// Conversion ratio from degrees to radians.
        /// </summary>
        public const double DegToRad = Math.PI / 180D;
        /// <summary>
        /// Conversion ratio from radians to degrees.
        /// </summary>
        public const double RadToDeg = 180D / Math.PI;

        /// <summary>
        /// Clamps ("sticks") a value between two others.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IComparable"/> values.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimal value.</param>
        /// <param name="max">The maximal value.</param>
        /// <returns>The clamped value.</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable
        {
            return value.CompareTo(min) < 0 ? min : (value.CompareTo(max) > 0 ? max : value);
        }

        /// <summary>
        /// Retuns the maximal value between two values.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IComparable"/> values.</typeparam>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>The maximum value between both values.</returns>
        public static T Max<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        /// <summary>
        /// Retuns the minimal value between two values.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IComparable"/> values.</typeparam>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>The minimal value between both values.</returns>
        public static T Min<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        /// <summary>
        /// Get the index of a 1D array treated as 2D
        /// </summary>
        /// <param name="x">The X coordinate of the 2D space.</param>
        /// <param name="y">The Y coordinate of the 2D space.</param>
        /// <param name="width">The width of the 2D space.</param>
        /// <returns>The 1D mapped index/</returns>
        public static int Flatten2D(int x, int y, int width)
        {
            return x + y * width;
        }

        /// <summary>
        /// Get 2D index from 1D index and with.
        /// </summary>
        /// <param name="index">The 1D index of the array.</param>
        /// <param name="width">The width of the 2D space.</param>
        /// <param name="x">The X coordinate of the 2D space.</param>
        /// <param name="y">The Y coordinate of the 2D space.</param>
        public static void FlatTo2D(int index, int width, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }
        
        
        public static int Flatten3D(int x, int y, int z, int width, int height)
        {
            return x + width*y + width*height*z;
        }


        public static void FlatTo3D(int index, int width, int height, out int x, out int y, out int z)
        {
            x = index % width;
            y = (index / width)%height;
            z = index / (width*height);
        }


        #region CopySign
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static Int64 CopySign(Int64 receiver, Int64 sender)
        {
            //if receiver negative
            if (receiver < 0L)
            {
                //if sender negative
                if (sender < 0L)
                {
                    return receiver;
                }

                //else return positive receiver
                return receiver * -1L;
            }

            //if sender negative
            if (sender < 0L)
            {
                //return negative receiver
                return receiver * -1L;
            }

            //else return positive receiver
            return receiver;
        }
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static Int32 CopySign(Int32 receiver, Int32 sender)
        {
            //if receiver negative
            if (receiver < 0)
            {
                //if sender negative
                if (sender < 0)
                {
                    return receiver;
                }

                //else return positive receiver
                return receiver * -1;
            }

            //if sender negative
            if (sender < 0)
            {
                //return negative receiver
                return receiver * -1;
            }

            //else return positive receiver
            return receiver;
        }
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static Int16 CopySign(Int16 receiver, Int16 sender)
        {
            //if receiver negative
            if (receiver < 0)
            {
                //if sender negative
                if (sender < 0)
                {
                    return receiver;
                }

                //else return positive receiver
                return (short)(receiver * -1);
            }

            //if sender negative
            if (sender < 0)
            {
                //return negative receiver
                return (short)(receiver * -1);
            }

            //else return positive receiver
            return receiver;
        }
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static decimal CopySign(decimal receiver, decimal sender)
        {
            //if receiver negative
            if (receiver < 0.0M)
            {
                //if sender negative
                if (sender < 0.0M)
                {
                    return receiver;
                }

                //else return positive receiver
                return receiver * -1M;
            }

            //if sender negative
            if (sender < 0.0M)
            {
                //return negative receiver
                return receiver * -1M;
            }

            //else return positive receiver
            return receiver;
        }
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static float CopySign(float receiver, float sender)
        {
            if (Single.IsNaN(receiver)) return Single.NaN;
            //if receiver negative
            if (receiver < 0.0F)
            {
                //if sender negative
                if (sender < 0.0F)
                {
                    return receiver;
                }

                //else return positive receiver
                return receiver * -1.0F;
            }

            //if sender negative
            if (sender < 0.0F)
            {
                //return negative receiver
                return receiver * -1.0F;
            }

            //else return positive receiver
            return receiver;
        }
        /// <summary>
        /// Copies the sign of a sender value to receiver one. 
        /// </summary>
        /// <param name="receiver">The value receiving the sign.</param>
        /// <param name="sender">The value sending the sign.</param>
        /// <returns>The receiver containing the sender sign.</returns>
        public static double CopySign(double receiver, double sender)
        {
            if (Double.IsNaN(receiver)) return Double.NaN;
            //if receiver negative
            if (receiver < 0.0D)
            {
                //if sender negative
                if (sender < 0.0D)
                {
                    return receiver;
                }

                //else return positive receiver
                return receiver * -1.0D;
            }

            //if sender negative
            if (sender < 0.0D)
            {
                //return negative receiver
                return receiver * -1.0D;
            }

            //else return positive receiver
            return receiver;
        }
        #endregion

        /// <summary>
        /// Remaps (~move) a value from a previous low / high cap to a new one (i.e. 5 from 0..10 remapped to 0..5 will give 2.5)
        /// </summary>
        /// <param name="value">The value to remap.</param>
        /// <param name="oldLow">The previous low-point of the range.</param>
        /// <param name="oldHigh">The previous high-point of the range.</param>
        /// <param name="newLow">The new low-point of the range.</param>
        /// <param name="newHigh">The new high-point of the range.</param>
        /// <returns>The remapped value.</returns>
        public static float Remap(float value, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
        }
        /// <summary>
        /// Remaps (~move) a value from a previous low / high cap to a new one (i.e. 5 from 0..10 remapped to 0..5 will give 2.5)
        /// </summary>
        /// <param name="value">The value to remap.</param>
        /// <param name="oldLow">The previous low-point of the range.</param>
        /// <param name="oldHigh">The previous high-point of the range.</param>
        /// <param name="newLow">The new low-point of the range.</param>
        /// <param name="newHigh">The new high-point of the range.</param>
        /// <returns>The remapped value.</returns>
        public static double Remap(double value, double oldLow, double oldHigh, double newLow, double newHigh)
        {
            return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
        }
        /// <summary>
        /// Remaps (~move) a value from a previous low / high cap to a new one (i.e. 5 from 0..10 remapped to 0..5 will give 2.5)
        /// </summary>
        /// <param name="value">The value to remap.</param>
        /// <param name="oldLow">The previous low-point of the range.</param>
        /// <param name="oldHigh">The previous high-point of the range.</param>
        /// <param name="newLow">The new low-point of the range.</param>
        /// <param name="newHigh">The new high-point of the range.</param>
        /// <returns>The remapped value.</returns>
        public static Vector2F Remap(Vector2F value, Vector2F oldLow, Vector2F oldHigh, Vector2F newLow, Vector2F newHigh)
        {
            return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
        }
        /// <summary>
        /// Remaps (~move) a value from a previous low / high cap to a new one (i.e. 5 from 0..10 remapped to 0..5 will give 2.5)
        /// </summary>
        /// <param name="value">The value to remap.</param>
        /// <param name="oldLow">The previous low-point of the range.</param>
        /// <param name="oldHigh">The previous high-point of the range.</param>
        /// <param name="newLow">The new low-point of the range.</param>
        /// <param name="newHigh">The new high-point of the range.</param>
        /// <returns>The remapped value.</returns>
        public static Vector3F Remap(Vector3F value, Vector3F oldLow, Vector3F oldHigh, Vector3F newLow, Vector3F newHigh)
        {
            return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
        }
        
        
        public static double DeltaAngle(double current, double target)
        {
            double delta = WMath.Repeat((target - current), 360.0F);
            if (delta > 180.0F)
                delta -= 360.0F;
            return delta;
        }
        
        public static double Repeat(double t, double length)
        {
            return Clamp(t - Math.Floor(t / length) * length, 0.0D, length);
        }

    }
}
