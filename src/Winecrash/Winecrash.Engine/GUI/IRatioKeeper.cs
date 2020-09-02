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

        public double Ratio { get; }

        public double GlobalRatio { get; }

        public double SizeX {get;}
        public double SizeY {get;}
    }
}
