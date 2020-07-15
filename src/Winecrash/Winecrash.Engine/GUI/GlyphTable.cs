using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Winecrash.Engine.GUI
{
    public sealed class GlyphTable
    {
        public string MapPath { get; set; }
        private Glyph[] Table;


        public Glyph this[char character]
        {
            get
            {
                return Table[CharIndices[character]];
            }
        }

        public Glyph[] this[string text]
        {
            get
            {
                if (String.IsNullOrWhiteSpace(text))
                { 
                    return Array.Empty<Glyph>(); 
                }

                Glyph[] glyphs = new Glyph[text.Length];

                for (int i = 0; i < text.Length; i++)
                {
                    glyphs[i] = this[text[i]];
                }

                return glyphs;
            }
        }

        

        public Texture Map { get; set; } = null;

        

        private Dictionary<char, int> CharIndices = null;

        public string Set { get; private set; } =
            "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz" + // Standard Letters
            "ÁáÀaÂâÄäÃãÅåÆæ" +      //Accents A
            "Çç" +                  //Accents C
            "ÉéÈèÊêËë" +            //Accents E
            "Ññ" +                  //Accents N
            "ÓóÒòÔôÖöÕõØøŒœ" +      //Accents O
            "ß" +                   //Accents S
            "ÚúÙùÛûÜü" +            //Accents U
            "0123456789";           //Numbers

        public GlyphTable(Texture map, Glyph[] table) 
        {
            this.Map = map;

            this.Table = table;

            BuildData();
        }

        [JsonConstructor]
        private GlyphTable(string MapPath, Glyph[] Table)
        {
            this.MapPath = MapPath;

            Viewport.DoOnce += () => this.Map = new Texture(MapPath);

            this.Table = Table;

            BuildData();
        }

        private void BuildData()
        {
            CharIndices = new Dictionary<char, int>(Table.Length);
            Set = "";

            Char character = ' ';

            for (int i = 0; i < this.Table.Length; i++)
            {
                character = this.Table[i].Character;
                Set += character;
                CharIndices.Add(character, i);
            }
        }
    }
}
