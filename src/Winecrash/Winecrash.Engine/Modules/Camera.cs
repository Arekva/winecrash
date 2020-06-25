using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public sealed class Camera : Module
    {
        internal static List<Camera> Cameras { get; set; } = new List<Camera>(1);

        protected internal override void Creation()
        {
            Cameras.Add(this);
        }
        protected internal override void OnDelete()
        {
            Cameras.Remove(this);
        }

        internal void OnRender()
        {

        }
    }
}
