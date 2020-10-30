using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;
using PixelFormat = System.Drawing.Imaging.PixelFormat;


namespace WEngine
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
            if (Engine.DoGUI)
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
            else
            {
                Debug.LogWarning("Unable to apply a texture while being no GUI.");
            }
        }

        public void WriteToStream(Stream saveStream)
        {
            if (this.Deleted) return;
            
            saveStream?.Write(this.Data, 0, this.Data.Length);
        }

        public unsafe void Save(string path)
        {
            if (this.Deleted || this.Data == null) return;

            using (Bitmap bm = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = bm.LockBits(
                    new Rectangle(0, 0, this.Width, this.Height),
                    ImageLockMode.WriteOnly,
                    bm.PixelFormat);

                uint* byteData = (uint*) bmpData.Scan0;

                fixed (byte* dataPtr = this.Data) //RGBA => ABGR
                {
                    uint* longDataPtr = (uint*) dataPtr;
                    
                    Parallel.For(0, this.Size.X * this.Size.Y, (i) =>
                    {
                        int j = (this.Size.X * this.Size.Y - 1) - i;
                        //    alpha and green are ok                     blue and red have to be switched
                        byteData[i] = longDataPtr[j] & 0xFF00FF00 | (longDataPtr[j] & 0x00FF0000) >> 16 | (longDataPtr[j] & 0x000000FF) << 16;
                    });
                }
                
                bm.UnlockBits(bmpData);
                
                bm.Save(path);
            }
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
        internal static object CacheLocker { get; set; } = new object();

        public static Texture Find(string name)
        {
            lock(CacheLocker)
                return Cache?.FirstOrDefault(t => t.Name == name);
        }

        public static Texture Blank { get; set; }

        static Texture()
        {
            if(Blank != null)
            {
                return;
            }

            Blank = new Texture(1, 1)
            {
                Name = "Blank",
                Size = new Vector2I(1, 1)
            };

            byte[] blank = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                blank[i] = 255;
            }

            Blank.Data = blank;

            Blank.Apply();
        }

        public Texture(int width, int height)
        {
            this.Name = "Texture";
            this.Size = new Vector2I(width, height);
            this.Data = new byte[4 * width * height];

            void genGL()
            {
                this.Handle = GL.GenTexture();
                this.Use();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            if (Thread.CurrentThread == Graphics.Window.Thread)
            {
                genGL();
            }
            else
            {
                Graphics.Window.InvokeRender(genGL);
            }

            lock(CacheLocker)
                Cache.Add(this);
        }

        public static Texture GetOrCreate(string path, bool clearOnApply = false)
        {
            string name = path.Split('/', '\\').Last().Split('.')[0];
            Texture tex = Texture.Find(name);
            if(tex == null)
            {
                tex = new Texture(path, name, clearOnApply);
            }

            return tex;
        }

        public unsafe Texture(string path, string name = null, bool clearOnApply = false) : base(name)
        {
            if(!File.Exists(path))
            {
                Debug.LogWarning($"Unable to load texture from \"{path}\": no file existing there.");
                this.Delete();
                return;
            }

            if (name == null)
                this.Name = path.Split('/', '\\').Last().Split('.')[0];

            using (Bitmap img = new Bitmap(path))
            {
                BitmapData data = img.LockBits(
                    new Rectangle(0, 0, img.Width, img.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                uint* byteData = (uint*)data.Scan0;
                this.Size = new Vector2I(img.Width, img.Height);
                this.Data = new byte[4 * this.Size.X * this.Size.Y];

                Parallel.For(0, this.Size.X * this.Size.Y, (i) => byteData[i] = (byteData[i] & 0xFF000000) >> 24 | (byteData[i] & 0x00FFFFFF) << 8);

                Marshal.Copy(data.Scan0, this.Data, 0, this.Data.Length);

                this.Data = this.Data.Reverse().ToArray();
            }

            lock(CacheLocker)
                Cache.Add(this);

            void glApply()
            {
                bool clearApply = clearOnApply;

                this.Handle = GL.GenTexture();

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

                if (clearApply)
                {
                    this.Data = null;
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            if (Thread.CurrentThread == Graphics.Window.Thread)
            {
                glApply();
            }

            else
            {
                Graphics.Window.InvokeRender(glApply);
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
            lock(CacheLocker)
                Cache.Remove(this);

            if (Engine.DoGUI)
            {
                GL.DeleteTexture(Handle);
                this.Handle = -1;
            }

            this.Data = null;

            base.Delete();
        }
    }
}
