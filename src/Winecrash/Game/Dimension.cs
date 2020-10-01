using System;
using System.Collections.Generic;
using WEngine;

namespace Winecrash
{
    public class Dimension : IEquatable<Dimension>
    {
        public string Identifier { get; }
        
        public Dimension(string identifier)
        {
            this.Identifier = identifier;
        }

        public override bool Equals(object obj)
        {
            return obj is Dimension dim && Equals(dim);
        }

        public bool Equals(Dimension other)
        {
            return other.Identifier == this.Identifier;
        }
    }
}