using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEngine
{
    public class EngineException : Exception
    {
        public EngineException(string message) : base(message) { }
    }
}
