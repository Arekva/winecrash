using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine.GUI
{
    public sealed class LabelRenderer : GUIRenderer
    {
        public Label Label { get; set; }

        protected internal override void Creation()
        {
            this.UseMask = false;

            base.Creation();
        }

        internal override void Use(Camera sender)
        {
            if (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0 || Deleted || Label == null || Material == null || Label.Text == null) return;

            Matrix4 transform;
            Quaternion rot = this.WObject.Rotation;
            
            Vector3F extents = this.Label.GlobalScale / 2.0F;
            Vector3F middle = this.Label.GlobalPosition;


            float fontSize = Label.FontSize;
            if (Label.AutoSize)
            {
                fontSize = (extents.Y * 2.0F) / Label.Lines.Length;
            }

            float wordSpace = (Label.WordSpace * fontSize);
            float lineSpace = (Label.LineSpace * fontSize) / (Label.Lines.Length * 0.5F);
            float totalYSize = Label.LineSpace * fontSize * Label.Lines.Length;

            int xDir = 0, yDir = 0;

            if (Label.Aligns.HasFlag(TextAligns.Left))
            {
                xDir += 1;
            }
            if (Label.Aligns.HasFlag(TextAligns.Right))
            {
                xDir -= 1;
            }

            if (Label.Aligns.HasFlag(TextAligns.Up))
            {
                yDir -= 1;
            }

            if (Label.Aligns.HasFlag(TextAligns.Down))
            {
                yDir += 1;
            }

            for (int i = 0; i < this.Label.Lines.Length; i++)
            {
                string str = this.Label.Lines[i];

                Glyph[] glyphs = Label.FontFamilly.Glyphs[str];

                Mesh[] meshes = Label.FontFamilly.Glyphs.GetMeshes(str);


                float lineXSize = 0.0F;

                for (int j = 0; j < glyphs.Length; j++)
                {
                    if (str[j] == ' ')
                    {
                        lineXSize += wordSpace;
                        continue;
                    }

                    float glyphratio = (float)glyphs[j].Width / (float)glyphs[j].Height;
                    lineXSize += glyphratio * fontSize;
                }

                float lineXStartPos, lineYPos;

                if (xDir == 1) lineXStartPos = extents.X; // left: -extents x
                else if (xDir == -1) lineXStartPos = -extents.X + lineXSize; //right: starts at extents x
                else lineXStartPos = lineXSize / 2.0F; //center: starts at -half x size

                if (yDir == -1) lineYPos = extents.Y - lineSpace * (i + 1); // up: starts at extents y
                else if (yDir == 1) lineYPos = -extents.Y + lineSpace * (this.Label.Lines.Length - i) - (lineSpace * 0.5F); // down: starts at -extents y ((length - 1) - i : go top to down and not down to top.
                else lineYPos = WMath.Remap((1.0F -(((float)i + 1) / ((float)this.Label.Lines.Length))), 0.0F, 1.0F, -totalYSize * 0.5F, totalYSize * 0.5F);

                float shiftX = 0.0F;
                /*if (xDir == 1) shiftX = 0.0F;
                else if (xDir == -1) shiftX = (extents.X * 2.0F) - */

                Vector3F trans;

                void Render(int index)
                {
                    trans = new Vector3F(middle.X + lineXStartPos - shiftX, middle.Y + lineYPos, middle.Z);

                    transform =
                    (Matrix4.CreateScale(fontSize, fontSize, fontSize) *
                    Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
                    Matrix4.CreateTranslation(trans.X, trans.Y, trans.Z) *
                    Matrix4.Identity) *
                    sender.ViewMatrix * sender.ProjectionMatrix;

                    GL.BindVertexArray(meshes[index].VertexArrayObject);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, meshes[index].VertexBufferObject);

                    this.Material.SetData<Matrix4>("transform", transform);
                    this.Material.Use();

                    GL.Disable(EnableCap.DepthTest);

                    GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)meshes[index].Indices, DrawElementsType.UnsignedInt, 0);
                }


                for (int j = 0; j < glyphs.Length; j++)
                {
                    if (str[j] == ' ')
                    {
                        shiftX += wordSpace;
                        continue;
                    }

                    float glyphratio = (float)glyphs[j].Width / (float)glyphs[j].Height;
                    shiftX += glyphratio * fontSize;

                    Render(j);
                }

            }


            /*float fontSize = Label.FontSize;

            if (Label.AutoSize)
            {
                fontSize = (extents.Y * 2.0F) / Label.Lines;
            }

            float shiftX = 0;
            float shiftY = fontSize;

            float space = (Label.WordSpace * fontSize);
            float linespace = (Label.LineSpace * fontSize) / (Label.Lines * 0.5F);

            float scale = fontSize;

            string txt = Label.Text;

            float glyphratio;

            Vector3F trans;

            for (int i = 0; i < glyphs.Length; i++)
            {
                if (txt[i] == ' ')
                {
                    shiftX += space;
                    continue;
                }
                else if (i + 1 < glyphs.Length && txt[i] == '\r' && txt[i + 1] == '\n')
                {
                    shiftY += linespace;
                    shiftX = 0;
                    continue;
                }
                else if (txt[i] == '\n')
                {
                    shiftY += linespace;
                    shiftX = 0;
                    continue;
                }

                glyphratio = (float)glyphs[i].Width / (float)glyphs[i].Height;
                shiftX += glyphratio * fontSize;

                if (shiftX > extents.X * 2)
                {
                    shiftX = glyphratio * fontSize;
                    shiftY += linespace;
                }

                if (shiftY > extents.Y * 2)
                {
                    break;
                }

                trans = new Vector3F((middle.X + extents.X) - shiftX, (middle.Y + extents.Y) - shiftY, middle.Z);

                

                transform =
                    (Matrix4.CreateScale(scale, scale, scale) *
                    Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
                    Matrix4.CreateTranslation(trans.X, trans.Y, trans.Z) *
                    Matrix4.Identity) *
                    sender.ViewMatrix * sender.ProjectionMatrix;

                GL.BindVertexArray(meshes[i].VertexArrayObject);
                GL.BindBuffer(BufferTarget.ArrayBuffer, meshes[i].VertexBufferObject);

                this.Material.SetData<Matrix4>("transform", transform);
                this.Material.Use();

                GL.Disable(EnableCap.DepthTest);

                GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)meshes[i].Indices, DrawElementsType.UnsignedInt, 0);
            }*/
        }

        protected internal override void OnDelete()
        {
            Label = null;

            base.OnDelete();
        }
    }
}
