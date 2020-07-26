using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace Winecrash.Engine.GUI
{
	public sealed class ImageRenderer : GUIRenderer
	{
		public Image Image { get; set; }

		private static Mesh _Panel;

		protected internal override void Creation()
		{
			this.UseMask = false;

			base.Creation();
		}

		internal override void Use(Camera sender)
		{
			if(!_Panel)
			{
				_Panel = new Mesh("Panel")
				{
					Vertices = new Vector3F[6]
					{
						new Vector3F(0.5F, 0.5F, 0),
						new Vector3F(0.5F, -0.5F, 0),
						new Vector3F(-0.5F, 0.5F, 0),
						new Vector3F(-0.5F, 0.5F, 0),
						new Vector3F(0.5F, -0.5F, 0),
						new Vector3F(-0.5F, -0.5F, 0)
					},
					Normals = new Vector3F[6]
					{
						Vector3F.Backward,
						Vector3F.Backward,
						Vector3F.Backward,
						Vector3F.Backward,
						Vector3F.Backward,
						Vector3F.Backward,
					},
					UVs = new Vector2F[6]
					{
						new Vector2F(1, 1),
						new Vector2F(1, 0),
						new Vector2F(0, 1),

						new Vector2F(0, 1),
						new Vector2F(1, 0),
						new Vector2F(0, 0),

					},

					Triangles = new uint[6] { 0, 1, 2, 3, 4, 5 }
				};

				_Panel.ApplySafe(true);
			}

			//if (!CheckValidity(sender)) return;
			if (!this.Enabled || (this.WObject.Layer & sender.RenderLayers) == 0 || Deleted || Image == null || Material == null) return;


			Vector3F tra = this.Image.GlobalPosition;
			Quaternion rot = this.Image.WObject.Rotation;
			Vector3F sca = this.Image.GlobalScale;

			Matrix4 transform =
				(Matrix4.CreateScale(sca.X, sca.Y, sca.Z) *
				Matrix4.CreateFromQuaternion(new OpenTK.Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W)) *
				Matrix4.CreateTranslation(tra.X, tra.Y, tra.Z) *
				Matrix4.Identity)
				* sender.ViewMatrix * sender.ProjectionMatrix;

			GL.BindVertexArray(_Panel.VertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _Panel.VertexBufferObject);

			//this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
			//this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
			//this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

			this.Material.SetData<Matrix4>("transform", transform);
			this.Material.Use();

			GL.Disable(EnableCap.DepthTest);

			GL.DrawElements(Wireframe ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Panel.Indices, DrawElementsType.UnsignedInt, 0);
		}
		protected internal override void OnDelete()
		{
			Image = null;

			base.OnDelete();
		}
	}
}
