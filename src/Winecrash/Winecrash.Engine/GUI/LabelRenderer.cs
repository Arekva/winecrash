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
                fontSize = WMath.Min((extents.Y * 2.0F) / Label.Lines.Length, float.PositiveInfinity); //todo: min between max height and max width
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

            float dst = Label.ShadowDistance;
            dst *= fontSize;

            Vector3F globalShift = new Vector3F(-dst, -dst, 0.0F);
            Color256 shadColr = Label.Color * 0.33D;
            Color256 col = new Color256(shadColr.R, shadColr.G, shadColr.B, 0.75D);

            void RenderLabel()
            {
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

                    if (yDir == -1) lineYPos = extents.Y - fontSize * (i + 1);
                    else if (yDir == 1) lineYPos = -extents.Y + fontSize * ((this.Label.Lines.Length - 1) - i);
                    else lineYPos = WMath.Remap((1.0F - (((float)i + 1) / ((float)this.Label.Lines.Length))), 0.0F, 1.0F, -totalYSize * 0.5F, totalYSize * 0.5F);

                    float shiftX = 0.0F;
                    /*if (xDir == 1) shiftX = 0.0F;
                    else if (xDir == -1) shiftX = (extents.X * 2.0F) - */

                    Vector3F trans;

                    void Render(int index)
                    {
                        trans = new Vector3F(middle.X + lineXStartPos - shiftX, middle.Y + lineYPos, middle.Z) + globalShift;

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
            }

            if (Label.Shadows)
            {
                this.Material.SetData<OpenTK.Vector4>("color", col);
                RenderLabel();
            }

            globalShift = new Vector3F(0.0F);
            col = Label.Color;

            this.Material.SetData<OpenTK.Vector4>("color", col);

            RenderLabel();
        }

        protected internal override void OnDelete()
        {
            Label = null;

            base.OnDelete();
        }
    }
}
