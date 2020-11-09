using System;
using System.Text;

namespace WEngine.Dab
{
    public class ShaderException : Exception
    {
        public string File { get; }
        public uint Line { get; }
        public uint Row { get; }
        public uint ProblemLength { get; }
        public string LineSource { get; }
        public string HelperMessage { get; }
        public string Reason { get; }

        public static T Create<T>(string file, uint line, uint row, uint problemLength, string lineSource,
            string reason) where T : ShaderException
        {
            return (T)Activator.CreateInstance(typeof(T), BuildHelperMessage(file, line, row, problemLength, lineSource, reason));
        }

        public ShaderException(string message) : base(message) { }

        protected static string BuildHelperMessage(string file, uint line, uint row, uint problemLength, string lineSource,
            string reason)
        {
            StringBuilder builder = new StringBuilder();
            
            builder.Append(file + ":" + line + "," + row + " : " + reason + Environment.NewLine +
                           lineSource + Environment.NewLine);

            // add spaces
            for (int i = 0; i < row; i++)
            {
                builder.Append(" ");
            }
            
            for (int i = 0; i < problemLength - 1; i++)
            {
                builder.Append("~");
            }
            
            builder.Append("^");

            return builder.ToString();
        }
    }
}