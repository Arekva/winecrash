using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public delegate void UpdateEventHandler(UpdateEventArgs e);

    public class UpdateEventArgs : EventArgs
    {
        public double DeltaTime { get; }

        public UpdateEventArgs(double deltaTime)
        {
            this.DeltaTime = deltaTime;
        }
    }
}
