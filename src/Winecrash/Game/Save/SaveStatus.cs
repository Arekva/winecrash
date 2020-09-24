using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash
{
    [Flags]
    public enum SaveStatus
    {
        None = 0,
        Created = 1,
        Generated = 2,
        Sane = 4,
        Corrupted = 8,
        Unknown = 16,
        OutOfDate = 32

    }
}
