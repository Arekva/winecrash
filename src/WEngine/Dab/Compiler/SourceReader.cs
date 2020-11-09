using System;
using System.IO;

namespace WEngine.Dab
{
    public class SourceReader : IDisposable
    {
        public string Content { get; }

        public Token Token { get; private set; } = new Token("TODO");

        private int _Index = 0;

        public SourceReader(string source)
        {
            Content = source;
        }

        // Go to next token
        public bool Read()
        {
            //TODO
            return false;
        }

        public void Dispose()
        {
        }
    }
}