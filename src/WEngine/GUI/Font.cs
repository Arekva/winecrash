using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using System.IO;

namespace WEngine.GUI
{
    public class Font : BaseObject
    {
        private static List<Font> _FontCache = new List<Font>();

        public GlyphTable Glyphs { get; } = null;

        public Font(string path, string name = null) : base()
        {
            this.Name = name ?? path.Split('/', '\\').Last().Split('.')[0];

            Glyphs = JsonConvert.DeserializeObject<GlyphTable>(File.ReadAllText(path));

            _FontCache.Add(this);
        }

        public Font(string name, GlyphTable glyphs) : base(name)
        {
            this.Glyphs = glyphs;
            _FontCache.Add(this);
        }

        public static Font Find(string name)
        {
            return _FontCache.FirstOrDefault(f => f.Name == name);
        }

        public override void Delete()
        {
            _FontCache.Remove(this);
            base.Delete();
        }
    }
}
