using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WEngine.Dab
{
    public class Source
    {
        internal const string Error =
            "shader Error { " +
                "pass Vertex { " +
                    "Vertex.Position = Output.Position = <Vertex.Position, 1.0F> * MVP.Transform;" +
                    "Vertex.Normal = Input.Normal.Normalize(); " +
                "} " +
                "pass Fragment { " +
                    "Output.Color = <1.0, 0.0, 1.0, 1.0>; " +
                "}" +
            " }";
        
        private string Raw { get; }
        
        public string Path { get; }
        public string FileFullName { get; }
        public string FileName { get; }
        public string FileExtention { get; }

        public Source(string path) : this(path, Encoding.UTF8) { }
        public Source(string path, Encoding encoding)
        {
            this.Path = path;
            
            if (path == null) throw new ArgumentNullException(nameof(path), "The shader source path is null.");
            if (!File.Exists(path)) throw new FileNotFoundException($"No shader source file found at \"{path}\".");

            this.FileFullName = path.Split('\\', '/').Last();
            
            StringBuilder builder = new StringBuilder();
            string[] fileNameDotSplit = this.FileFullName.Split('.');
            for (int i = 0; i < fileNameDotSplit.Length - 1; i++)
            {
                builder.Append(fileNameDotSplit[i]);

                if (i < fileNameDotSplit.Length - 2) builder.Append(".");
            }

            this.FileName = builder.ToString();
            this.FileExtention = fileNameDotSplit.Last();
            this.Raw = File.ReadAllText(path, encoding);
        }

        public string[] GetShadersSources()
        {
            // each shader starts with a shader keyword and stops when its last curly bracket has been closed.

            string[] lines = Raw.Split('\n');

            List<string> sources = new List<string>();
            StringBuilder builder = new StringBuilder();

            bool currentlyReadingActualShader = false;
            int bracketsDelta = 0;
            for (int line = 0, linesLength = lines.Length; line < linesLength; line++)
            {
                string trimmed = lines[line].Trim();

                if (trimmed.Length != 0)
                {
                    if (currentlyReadingActualShader)
                    {
                        foreach (char c in trimmed)
                        {
                            if (c == '{') bracketsDelta++;
                            else if (c == '}') bracketsDelta--;
                        }
                    }

                    if (bracketsDelta < 1 && currentlyReadingActualShader)
                    {
                        builder.Append(lines[line]);
                        currentlyReadingActualShader = false;
                        sources.Add(builder.ToString());
                        builder = new StringBuilder();
                    }

                    // if a new shader is declared
                    if (trimmed.Length >= "shader".Length && trimmed.Substring(0, "shader".Length) == "shader")
                    {
                        if (currentlyReadingActualShader)
                            throw ShaderException.Create<ShaderLoadException>(
                                this.FileFullName, (uint) line, (uint)lines[line].IndexOf("shader"), (uint) "shader".Length, lines[line],
                                "Nested shaders are not allowed. Please declare one shader at once.");

                        else
                        {
                            currentlyReadingActualShader = true;
                            
                            foreach (char c in trimmed)
                            {
                                if (c == '{') bracketsDelta++;
                                else if (c == '}') bracketsDelta--;
                            }
                        }
                    }

                    if (currentlyReadingActualShader)
                    {
                        builder.Append(lines[line] + "\n");
                    }
                }
            }

            return sources.ToArray();
        }
    }
}