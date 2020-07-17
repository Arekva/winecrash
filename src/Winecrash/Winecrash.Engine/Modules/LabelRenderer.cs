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

            /*this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
            this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
            this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);*/

            //Debug.Log(Material.Shader);
            
            Matrix4 transform;
            float charPos;
            Vector3F basePos = this.WObject.Position;
            Quaternion rot = this.WObject.Rotation;

            Vector3F sca = Vector3F.One * this.Label.FontSize;

            Mesh[] meshes = Label.FontFamilly.Glyphs.GetMeshes(Label.Text);

            GUIModule gui = this.Label.WObject.Parent.GetModule<GUIModule>();

            
            float minX, minY, maxX, maxY;


            float maxPosX = basePos.X + glyphs.Length * Label.FontSize + glyphs.Length * Label.InterCharacterSpace;

            for (int i = 0; i < glyphs.Length; i++)
            {
                charPos = basePos.X + i * Label.FontSize + i * Label.InterCharacterSpace;

                

                transform =
                    (Matrix4.CreateScale(sca.X, sca.Y, sca.Z) *
                    Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
                    Matrix4.CreateTranslation(maxPosX - charPos, basePos.Y, basePos.Z) *
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
                //GL.DepthMask(this.UseMask);


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
