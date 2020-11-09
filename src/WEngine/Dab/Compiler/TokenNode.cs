using System.Collections.Generic;

namespace WEngine.Dab
{
    public class TokenNode
    {
        public Token Token { get; }
        
        public List<TokenNode> Subnodes { get; set; } = new List<TokenNode>();
        
        public TokenNode(Token token)
        {
            this.Token = token;
        }
    }
}