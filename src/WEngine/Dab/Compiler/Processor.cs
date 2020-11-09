using System;
using System.Text;
using System.Windows.Forms;

namespace WEngine.Dab
{
    public class Processor
    {
        /// <summary>
        /// The source as passed into the Processor constructor.
        /// </summary>
        public string SourceRaw { get; }
        
        /// <summary>
        /// The clean up source: no new lines, no tabs nor useless whitespaces.
        /// </summary>
        public string SourceClean { get; }
        
        public Processor(string source)
        {
            this.SourceRaw = source;
            this.SourceClean = CleanupSource(source);
        }

        private string CleanupSource(string source)
        {
            StringBuilder commentCleaner = new StringBuilder(source.Length);

            string[] sourceLines = source.Trim().Split('\r','\n');
            
            // remove comments
            char previousChar = ' ';
            char currentChar;
            bool multilineComment = false;
            for (int i = 0; i < sourceLines.Length; i++)
            {
                string line = sourceLines[i];
                
                StringBuilder lineBuilder = new StringBuilder(line.Length);
                
                for (int j = 0; j < line.Length; j++)
                {
                    currentChar = line[j];

                    if (currentChar == '/' && previousChar == '/') // line comment
                    {
                        // remove last line that was a /
                        lineBuilder.Remove(lineBuilder.Length - 1, 1);
                        break; // stop building this line
                    }
                    else if (multilineComment  && previousChar == '*' && currentChar == '/')
                    {
                        multilineComment = false;
                        previousChar = ' '; // clear previous char
                        continue;
                    }
                    else if (!multilineComment && previousChar == '/' && currentChar == '*')
                    {
                        multilineComment = true;
                        lineBuilder.Remove(lineBuilder.Length - 1, 1);
                        previousChar = ' '; // clear previous char
                        continue;
                    }
                    else if (!multilineComment) // otherwise just add char
                    {
                        lineBuilder.Append(currentChar);
                    }
                    
                    previousChar = currentChar;
                }

                if(!string.IsNullOrWhiteSpace(lineBuilder.ToString()) && !string.IsNullOrEmpty(lineBuilder.ToString()))
                    commentCleaner.Append(lineBuilder);
            }
            // remove tabs
            commentCleaner.Replace("\t", " ");
            
            StringBuilder spacesCleaner = new StringBuilder(commentCleaner.Length);
            
            // remove useless whitespaces
            bool inSpaces = false;
            for (int i = 0; i < commentCleaner.Length; i++)
            {
                Char c = commentCleaner[i];
                if (inSpaces)
                {
                    if (c != ' ')
                    {
                        inSpaces = false;
                        spacesCleaner.Append(c);
                    }
                }
                else if (c == ' ')
                {
                    inSpaces = true;
                    spacesCleaner.Append(' ');
                }
                else
                {
                    spacesCleaner.Append(c);
                }
            }

            return spacesCleaner.ToString();
        }

        public TokenNode CreateTree()
        {
            TokenNode main = new TokenNode(null);

            return main;
        }
    }
}