using System.Collections.Generic;

namespace WEngine
{
    public sealed class PointLight : Light
    {
        public double Range { get; set; } = 1.0D;

        internal static List<PointLight> PointLights { get; private set; } = new List<PointLight>();

        public double GetIntensity(double distance)
        {
            if (distance > Range) return 0.0D;

            return ((1.0D / distance * distance) * Range) * Intensity;
        }

        public double GetIntensity(Vector3D point)
        {
            return GetIntensity(Vector3D.Distance(point, this.WObject.Position));
        }

        protected internal override void Creation()
        {
            PointLights.Add(this);

            base.Creation();
        }

        protected internal override void OnDelete()
        {
            PointLights.Remove(this);

            base.OnDelete();
        }
    }
}
