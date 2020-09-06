using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Threading;

namespace Winecrash.Engine
{
    public sealed class Texture : BaseObject
    {
        internal int Handle { get; private set; }

        internal byte[] Data { get; set; }

        public void SetPixel(int x, int y, Color32 color)
        {
            int i = (x + Size.X * y) * 4;

            this.Data[i] = color.R;
            this.Data[i + 1] = color.G;
            this.Data[i + 2] = color.B;
            this.Data[i + 3] = color.A;
        }

        public Color32 GetPixel(int x, int y)
        {
            int i = (x + Size.X * y) * 4;

            Color32 col = new Color32
            {
                R = this.Data[i],
                G = this.Data[i + 1],
                B = this.Data[i + 2],
                A = this.Data[i + 3]
            };
            return col;
        }

        public void SetPixels(int x, int y, int width, int height, Color32[] colors)
        {
                if (colors == null) return;

                int iColor = 0;
                for (int texy = 0; texy < height; texy++)
                {
                    for (int texx = 0; texx < width; texx++)
                    {
                        int i = ((x + texx) + Size.X * (y + texy)) * 4;

                        this.Data[i] = colors[iColor].R;
                        this.Data[i + 1] = colors[iColor].G;
                        this.Data[i + 2] = colors[iColor].B;
                        this.Data[i + 3] = colors[iColor].A;

                        iColor++;
                    }
                }
        }

        public Color32[] GetPixels(int x, int y, int width, int height)
        {
            Color32[] colors = new Color32[width * height];

            int iColor = 0;
            for (int texy = 0; texy < height; texy++)
            {
                for (int texx = 0; texx < width; texx++)
                {
                    int i = ((x + texx) + Size.X * (y + texy)) * 4;


                    colors[iColor] = new Color32(this.Data[i], this.Data[i + 1], this.Data[i + 2], this.Data[i + 3]);

                    iColor++;
                }
            }

            return colors;
        }

        public void Apply()
        {
            Graphics.Window.InvokeRender(() =>
            {
                this.Use();

                GL.TexImage2D(TextureTarget.Texture2D,
                            0,
                            PixelInternalFormat.Rgba,
                            this.Width,
                            this.Height,
                            0,
                            OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            this.Data);
            });
        }



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

        internal static List<Texture> Cache { get; set; } = new List<Texture>();

        public static Texture Find(string name)
        {
            return Cache?.FirstOrDefault(t => t.Name == name);
        }

        public static Texture Blank { get; set; }
        /// <summary>
        /// DO NOT USE ! USED BY ACTIVATOR.
        /// </summary>
        [Obsolete("Use Texture(string, string) instead.\nThis .ctor is meant to be used by Material's Activator")]
        public Texture()
        {
            if(Blank != null)
            {
                this.Delete();
                return;
            }

            Blank = this;

            this.Name = "Blank";
            this.Size = new Vector2I(1, 1);
            byte[] blank = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                blank[i] = 255;
            }

            this.Data = blank;

            this.Handle = GL.GenTexture();

            this.Use();

            GL.TexImage2D(TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        this.Width,
                        this.Height,
                        0,
                        OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        PixelType.UnsignedByte,
                        blank);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            Blank = this;
            Cache.Add(this);
        }

        public Texture(int width, int height)
        {
            this.Name = "Texture";
            this.Handle = GL.GenTexture();

            this.Use();

            this.Size = new Vector2I(width, height);
            this.Data = new byte[4 * width * height];

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            Cache.Add(this);
        }

        public static Texture GetOrCreate(string path)
        {
            string name = path.Split('/', '\\').Last().Split('.')[0];
            Texture tex = Texture.Find(name);
            if(tex == null)
            {
                tex = new Texture(path, name);
            }

            return tex;
        }

        public Texture(string path, string name = null) : base(name)
        {
            if(!File.Exists(path))
            {
                Debug.LogWarning($"Unable to load mesh from \"{path}\": no file existing there.");
                this.Delete();
                return;
            }

            Func<bool> del = new Func<bool>(() =>
            {
                if (name == null)
                    this.Name = path.Split('/', '\\').Last().Split('.')[0];

                this.Handle = GL.GenTexture();

                this.Use();

                unsafe
                {
                    using (Bitmap img = new Bitmap(path))
                    {
                        BitmapData data = img.LockBits(
                            new Rectangle(0, 0, img.Width, img.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                        uint* byteData = (uint*)data.Scan0;
                        this.Data = new byte[4 * img.Width * img.Height];

                        for (int i = 0; i < img.Width * img.Height; i++)
                        {
                            //reverse red and blue: bgra to rgba
                            byteData[i] = (byteData[i] & 0x000000FF) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);

                            //in order to reverse to array (flip upside down)
                            byteData[i] = (byteData[i] & 0x000000FF) << 24 | (byteData[i] & 0x0000FF00) << 8 | (byteData[i] & 0x00FF0000) >> 8 | (byteData[i] & 0xFF000000) >> 24;
                        }

                        Marshal.Copy(data.Scan0, this.Data, 0, this.Data.Length);

                        this.Data = this.Data.Reverse().ToArray();

                        GL.TexImage2D(TextureTarget.Texture2D,
                            0,
                            PixelInternalFormat.Rgba,
                            img.Width,
                            img.Height,
                            0,
                            OpenTK.Graphics.OpenGL4.PixelFormat.Rgba,
                            PixelType.UnsignedByte,
                            this.Data);

                        this.Size = new Vector2I(img.Width, img.Height);
                    }

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                    Cache.Add(this);
                }

                return true;
            });
            
            if(Thread.CurrentThread.Name == "OpenGL")
            {
                del.Invoke();
            }

            else
            {
                Graphics.Window.InvokeRender(() => del.Invoke());
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
            Cache.Remove(this);

            GL.DeleteTexture(Handle);
            this.Handle = -1;
            this.Data = null;

            base.Delete();
        }
    }
}
