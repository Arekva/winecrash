using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public interface IRatioKeeper
    {
        public bool KeepRatio { get; set; }

        public float Ratio { get; }
    }
}
