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

        static Mesh debugMesh = Mesh.LoadFile("assets/models/Cube.obj", MeshFormats.Wavefront);


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

        public Mesh[] GetMeshes(string text)
        {
            Mesh[] meshes = new Mesh[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                meshes[i] = CharactersMeshes[CharIndices[text[i]]];
            }

            return meshes;
        }

        internal Mesh[] CharactersMeshes;

        /*private static Vector3F[] CharacterVertices = new Vector3F[6] 
        {
            new Vector3F(1, 2f, 0),
            new Vector3F(1, 0, 0),
            new Vector3F(0, 2f, 0),
            new Vector3F(0, 2f, 0),
            new Vector3F(1, 0, 0),
            new Vector3F(0, 0, 0)
        };*/
        private static Vector3F[] CharacterNormals = new Vector3F[6]
        {
            Vector3F.Backward,
            Vector3F.Backward,
            Vector3F.Backward,
            Vector3F.Backward,
            Vector3F.Backward,
            Vector3F.Backward,
        };

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
            CharactersMeshes = new Mesh[this.Table.Length];
            Set = "";

            Char character = ' ';

            float xmin = 0.0F;
            float xmax = 1.0F;
            float ymin = 0.0F;
            float ymax = 1.0F;

            Vector2F[] uvs;

            for (int i = 0; i < this.Table.Length; i++)
            {
                character = this.Table[i].Character;
                Set += character;
                CharIndices.Add(character, i);

                int n = i;
                Viewport.DoOnce += () =>
                {
                    float texWidth = Map.Width;
                    float texHeight = Map.Height;

                    xmin = (texWidth - (this.Table[n].X + this.Table[n].Width)) / texWidth;
                    xmax = (texWidth - this.Table[n].X) / texWidth;

                    ymin = (texHeight - this.Table[n].Y) / texHeight;
                    ymax = (texHeight - (this.Table[n].Y - this.Table[n].Height)) / texHeight;

                    float ratio = this.Table[n].Width / (float)this.Table[n].Height;

                    Mesh m = CharactersMeshes[n] = new Mesh()
                    {
                        Vertices = new Vector3F[6] {
                            new Vector3F(ratio, 1, 0),
                            new Vector3F(ratio, 0, 0),
                            new Vector3F(0, 1, 0),
                            new Vector3F(0, 1, 0),
                            new Vector3F(ratio, 0, 0),
                            new Vector3F(0, 0, 0)
                        },
                        Normals = CharacterNormals,
                        UVs = new Vector2F[6]
                        {
                            new Vector2F(xmax, ymax),
                            new Vector2F(xmax, ymin),
                            new Vector2F(xmin, ymax),

                            new Vector2F(xmin, ymax),
                            new Vector2F(xmax, ymin),
                            new Vector2F(xmin, ymin),

                        },

                        Triangles = new uint[6] { 0, 1, 2, 3, 4, 5 }
                    };

                    m.ApplySafe(true);
                };
            }
        }
    }
}
