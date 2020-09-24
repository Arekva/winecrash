using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash
{
    /// <summary>
    /// Exception thrown when an error occurs on save operation.
    /// </summary>
    public class SaveException : Exception
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SaveException() : base() { }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SaveException(string message) : base(message) { }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SaveException(string message, Exception innerException) : base(message, innerException) { }
    }
}
