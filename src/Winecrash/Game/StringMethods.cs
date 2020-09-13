using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash
{
    public static class StringMethods
    {
        /// <summary>
        /// Replace numerous characters in a string by another string.
        /// </summary>
        /// <param name="s">The base work string.</param>
        /// <param name="separators">The wanted separators so replace.</param>
        /// <param name="newVal">The replacement of the separators.</param>
        /// <returns></returns>
        public static string Replace(this string s, char[] separators, string newVal)
        {
            if (separators == null || newVal == null) return s;

            string[] temp;

            temp = s.Split(separators);
            return String.Join(newVal, temp);
        }
    }
}
