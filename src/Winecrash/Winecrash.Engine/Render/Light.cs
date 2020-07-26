using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public abstract class Light : Module
    {
        public Color256 Color { get; set; } = new Color256(1.0D, 1.0D, 1.0D, 1.0D);
        public double Intensity { get; set; } = 1.0D;

        internal static List<Light> ActiveLights { get; private set; } = new List<Light>();
        internal static List<Light> Lights { get; private set; } = new List<Light>();

        protected internal override void Creation()
        {
            Lights.Add(this);
        }

        protected internal override void OnEnable()
        {
            ActiveLights.Add(this);
        }

        protected internal override void OnDisable()
        {
            ActiveLights.Remove(this);
        }

        protected internal override void OnDelete()
        {
            ActiveLights.Remove(this);
            Lights.Remove(this);
        }
    }
}
