using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine
{
    public interface IVectorable
    {
        int Dimensions { get; }

        double SquaredLength { get; }
        double Length { get; }
    }
}
