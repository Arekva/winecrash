namespace WEngine.Dab
{
    public class CompilerToken : Token
    {
        public bool CaseSensitive { get; set; } = true;
        
        public CompilerToken(string value) : base(value) { }
    }
}