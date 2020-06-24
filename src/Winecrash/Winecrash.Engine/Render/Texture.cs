using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Winecrash.Engine
{
    public class Texture : BaseObject
    {
        public byte[] Data { get; set; }

        public static Texture FromFile(string path)
        {
            Texture tex = new Texture();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Image img = Image.FromFile(path);
                    img.Save(ms, img.RawFormat);
                    tex.Data = ms.ToArray();
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Error when loading texture at " + path + " : " + e.Message);
            }

            return tex;
        }

        public override void Delete()
        {
            this.Data = null;

            base.Delete();
        }
    }
}
