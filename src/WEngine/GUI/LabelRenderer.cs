using System.Linq;

using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine.GUI
{
    public sealed class LabelRenderer : GUIRenderer
    {
        public Label Label { get; set; }

        protected internal override void Creation()
        {
            this.UseMask = false;

            base.Creation();
        }

        internal unsafe override void Use(Camera sender)
        {
            if (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0 || Deleted || Label == null || Material == null || Label.Text == null) return;

            Matrix4D transform;
            Quaternion rot = this.WObject.Rotation;

            Vector3D extents = this.Label.GlobalScale / 2.0D;
            Vector3D middle = this.Label.GlobalPosition;


            double fontSize = Label.FontSize;

            if (Label.AutoSize)
            {
                fontSize = WMath.Min((extents.Y * 2.0D) / Label.Lines.Length, double.PositiveInfinity); //todo: min between max height and max width
            }

            double wordSpace = (Label.WordSpace * fontSize);
            double lineSpace = (Label.LineSpace * fontSize) / (Label.Lines.Length * 0.5D);
            double totalYSize = Label.LineSpace * fontSize * Label.Lines.Length;

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

            double dst = Label.ShadowDistance;
            dst *= fontSize;

            Vector3D globalShift = new Vector3D(-dst, -dst, 0.0F);
            Color256 shadColr = Label.Color * 0.33D;
            Color256 col = new Color256(shadColr.R, shadColr.G, shadColr.B, 0.75D);

            
            int dummy = 0;

            Material.MaterialData matData = this.Material._Data.Where(d => d.Name == "transform").FirstOrDefault();

            if (matData == null) return;

            bool doRotate = Label.Rotation != 0.0D;

            Quaternion labelRotation = new Quaternion(Vector3D.Forward, Label.Rotation);

            void RenderLabel()
            {
                this.Material.Use();

                int count = 0;

                for (int i = 0; i < this.Label.Lines.Length; i++)
                {
                    string str = this.Label.Lines[i];

                    Glyph[] glyphs = Label.FontFamilly.Glyphs[str];

                    Mesh[] meshes = Label.FontFamilly.Glyphs.GetMeshes(str);


                    double lineXSize = 0.0D;

                    for (int j = 0; j < glyphs.Length; j++)
                    {
                        if (str[j] == ' ')
                        {
                            lineXSize += wordSpace;
                            continue;
                        }

                        double glyphratio = (double)glyphs[j].Width / (double)glyphs[j].Height;
                        lineXSize += glyphratio * fontSize;
                    }

                    double lineXStartPos, lineYPos;

                    if (xDir == 1) lineXStartPos = extents.X; // left: -extents x
                    else if (xDir == -1) lineXStartPos = -extents.X + lineXSize; //right: starts at extents x
                    else lineXStartPos = lineXSize / 2.0F; //center: starts at -half x size

                    if (yDir == -1) lineYPos = extents.Y - fontSize * (i + 1);
                    else if (yDir == 1) lineYPos = -extents.Y + fontSize * ((this.Label.Lines.Length - 1) - i);
                    else lineYPos = WMath.Remap((1.0D - (((double)i + 1) / ((double)this.Label.Lines.Length))), 0.0D, 1.0D, -totalYSize * 0.5D, totalYSize * 0.5D);

                    double shiftX = 0.0D;
                    /*if (xDir == 1) shiftX = 0.0F;
                    else if (xDir == -1) shiftX = (extents.X * 2.0F) - */

                    Vector3F trans;

                    void Render(int index)
                    {
                        trans = new Vector3D(middle.X + lineXStartPos - shiftX, middle.Y + lineYPos, middle.Z) + globalShift;


                        Quaternion rotation = rot;

                        if(doRotate)
                        {
                            rotation *= labelRotation;
                            trans = trans.RotateAround(middle, labelRotation);
                        }


                        transform =
                            new Matrix4D(new Vector3D(fontSize), true) *
                            new Matrix4D(rotation) *
                            new Matrix4D(trans, false) *
                            Matrix4D.Identity *


                        /*(Matrix4.CreateScale(fontSize, fontSize, fontSize) *
                        Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
                        Matrix4.CreateTranslation(trans.X, trans.Y, trans.Z) *
                        Matrix4.Identity) **/
                        sender.ViewMatrix * sender.ProjectionMatrix;

                        GL.BindVertexArray(meshes[index].VertexArrayObject);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, meshes[index].VertexBufferObject);

                        //matData.Data = transform;
                        //this.Material.SetData<Matrix4>("transform", transform);

                        //this.Material.SetGLData(ref matData, ref dummy);
                        this.Material.SetMatrix4(matData.Location, (Matrix4)transform);
                        this.Material.Shader.Use();
                        GL.Disable(EnableCap.DepthTest);

                        GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)meshes[index].Indices, DrawElementsType.UnsignedInt, 0);
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

                        //count++;
                    }

                }

                //GL.MultiDrawElementsIndirect((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, DrawElementsType.UnsignedInt, , , Shader.Size);
                //GL.DrawElementsInstanced((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, , 0);
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
