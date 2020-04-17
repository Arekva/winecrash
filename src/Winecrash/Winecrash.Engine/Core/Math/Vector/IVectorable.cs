using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public interface IVectorable
    {
        public int Dimensions { get; }

        public double SquaredLength { get; }
        public double Length { get; }
    }
}
