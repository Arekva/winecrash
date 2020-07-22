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
			if (Deleted || Material == null || _Mesh == null || _Mesh.Indices == null || _Mesh.ElementBufferObject == -1 || _Mesh.VertexArrayObject == -1 || _Mesh.VertexBufferObject == -1) return;

			Vector3F tra = this.Model.GlobalPosition;
			Quaternion rot = this.Model.WObject.Rotation;
			Vector3F sca = this.Model.GlobalScale;

			Matrix4 transform =
				(Matrix4.CreateScale(sca.X, sca.Y, sca.Z) *
				Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
				Matrix4.CreateTranslation(tra.X, tra.Y, tra.Z) *
				Matrix4.Identity)
				* sender.ViewMatrix * sender.ProjectionMatrix;

			GL.BindVertexArray(_Mesh.VertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _Mesh.VertexBufferObject);

			this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
			this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
			this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

			this.Material.SetData<Matrix4>("transform", transform);
			this.Material.Use();

			GL.Disable(EnableCap.DepthTest);

			GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Mesh.Indices, DrawElementsType.UnsignedInt, 0);
		}

		protected internal override void OnDelete()
		{
			Model = null;

			base.OnDelete();
		}
	}
}
