using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine
{
    public class Texture : BaseObject
    {
        internal int Handle { get; private set; }
        public Vector2I Size { get; private set; }
        public int Width
        {
            get
            {
                return this.Size.X;
            }
        }
        public int Height
        {
            get
            {
                return this.Size.Y;
            }
        }


        public Texture(string path) : base()
        {
            this.Handle = GL.GenTexture();

            this.Use();

            try
            {
                using (Bitmap img = new Bitmap(path))
                {
                    var data = img.LockBits(
                        new Rectangle(0, 0, img.Width, img.Height), 
                        ImageLockMode.ReadOnly, 
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.TexImage2D(TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        img.Width,
                        img.Height,
                        0,
                        OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        data.Scan0);

                    this.Size = new Vector2I(img.Width, img.Height);
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            catch(Exception e)
            {
                Debug.LogError("Error when loading texture at " + path + " : " + e.Message + "\n" + "Source: " + e.Source + "\n"  + e.StackTrace);
                this.Delete();
            }
        }

        internal void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            if(this.Deleted) return;

            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public override void Delete()
        {
            //this.Data = null;

            GL.DeleteTexture(Handle);
            this.Handle = -1;

            base.Delete();
        }
    }
}
