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
            if (Deleted || Label == null || Material == null) return;

            Glyph[] glyphs = Label.FontFamilly.Glyphs[Label.Text];

            Matrix4 transform;
            Quaternion rot = this.WObject.Rotation;
            Mesh[] meshes = Label.FontFamilly.Glyphs.GetMeshes(Label.Text);

            Vector3F extents = this.Label.GlobalScale / 2.0F;
            Vector3F middle = this.Label.GlobalPosition;

            float shiftX = 0;
            float shiftY = Label.FontSize;

            float space = Label.WordSpace * Label.FontSize;
            float linespace = Label.LineSpace * Label.FontSize;

            float scale = Label.FontSize;

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
                else if (txt[i] == '\n')
                {
                    shiftY += linespace;
                    shiftX = 0;
                    continue;
                }

                glyphratio = (float)glyphs[i].Width / (float)glyphs[i].Height;
                shiftX += glyphratio * Label.FontSize;

                if (shiftX > extents.X * 2)
                {
                    shiftX = glyphratio * Label.FontSize;
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

                this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
                this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
                this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

                this.Material.SetData<Matrix4>("transform", transform);

                this.Material.Use();

                GL.Enable(EnableCap.DepthTest);

                GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)meshes[i].Indices, DrawElementsType.UnsignedInt, 0);
            }
        }

        protected internal override void OnDelete()
        {
            Label = null;

            base.OnDelete();
        }
    }
}
