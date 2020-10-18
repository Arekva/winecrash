using System;

namespace WEngine
{
    public static class EGuid
    {
        public static Guid UniqueFromString(string str)
        {
            Random r = new Random(str.GetHashCode());

            byte[] bytes = new byte[16];
            r.NextBytes(bytes);
            
            return new Guid(bytes);
        }
    }
}