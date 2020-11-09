using System;
using System.Collections.Generic;
using WEngine.Dab;

namespace DabTester
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Running DAB unit tests.");
            Source src = new Source("test/Unlit.dab");

            string[] sources = null;
            try
            {
                sources = src.GetShadersSources();
            }
            catch (ShaderException se)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(se.Message);
                Console.ResetColor();
            }
            
            WEngine.Dab.Processor proc = new Processor(sources[0]);
            
                        
            Console.WriteLine(proc.SourceClean);
        }
    }
}