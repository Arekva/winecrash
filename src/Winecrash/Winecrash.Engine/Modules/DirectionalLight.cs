using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class DirectionalLight : Light
    {
        public static DirectionalLight Main { get; set; }
        internal static List<DirectionalLight> DirectionalLights { get; private set; } = new List<DirectionalLight>();

        public Color256 Ambient { get; set; } = new Color256(0.1D, 0.1D, 0.1D, 1.0D);

        protected internal override void Creation()
        {
            if(Main == null)
            {
                Main = this;
            }

            DirectionalLights.Add(this);

            base.Creation();
        }


        protected internal override void OnDelete()
        {
            if(Main == this)
            {
                Main = null;
            }
            DirectionalLights.Remove(this);
            base.OnDelete();
        }
    }
}
