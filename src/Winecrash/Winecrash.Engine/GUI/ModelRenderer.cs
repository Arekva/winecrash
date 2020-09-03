using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winecrash.Engine.GUI
{
    public class ModelRenderer : GUIRenderer
    {
		public Model Model { get; set; }

        protected internal override void Creation()
        {
            this.UseMask = false;

            base.Creation();
        }

        internal override void Use(Camera sender)
        {
			if (CheckValidity(sender)) return;

			Matrix4D transform =
				new Matrix4D(this.Model.GlobalScale, true) *
							new Matrix4D(this.Model.WObject.Rotation) *
							new Matrix4D(this.Model.GlobalPosition, false) *
							Matrix4D.Identity
				* sender.ViewMatrix * sender.ProjectionMatrix;

			GL.BindVertexArray(_Mesh.VertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _Mesh.VertexBufferObject);

			this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
			this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
			this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

			this.Material.SetData<Matrix4>("transform", (Matrix4)transform);
			this.Material.Use();

			GL.Disable(EnableCap.DepthTest);

			GL.DrawElements((Wireframe | Global_Wireframe ) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Mesh.Indices, DrawElementsType.UnsignedInt, 0);
		}

		protected internal override void OnDelete()
		{
			Model = null;

			base.OnDelete();
		}
	}
}
