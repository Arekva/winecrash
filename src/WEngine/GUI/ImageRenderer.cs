using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine.GUI
{
	public sealed class ImageRenderer : GUIRenderer
	{
		public Image Image { get; set; }

		private static Mesh _Panel;

		protected internal override void Creation()
		{
			this.UseMask = false;
			this.UseDepth = false;

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

			Matrix4D transform =
				new Matrix4D(sca, true) *
				new Matrix4D(rot) *
				new Matrix4D(tra, false) *
				Matrix4D.Identity;

			transform *= sender.ViewMatrix * sender.ProjectionMatrix;

			GL.BindVertexArray(_Panel.VertexArrayObject);
			GL.BindBuffer(BufferTarget.ArrayBuffer, _Panel.VertexBufferObject);

			//this.Material.Shader.SetAttribute("position", AttributeTypes.Vertice);
			//this.Material.Shader.SetAttribute("uv", AttributeTypes.UV);
			//this.Material.Shader.SetAttribute("normal", AttributeTypes.Normal);

			this.Material.SetData<Matrix4>("transform", (Matrix4)transform);
			
			this.PassTransformMatrices(transform, sender.ViewMatrix, sender.ProjectionMatrix);
			this.Material.Use();

			GL.Disable(EnableCap.DepthTest);

			GL.DrawElements((Wireframe | Global_Wireframe) ? PrimitiveType.LineLoop : PrimitiveType.Triangles, (int)_Panel.Indices, DrawElementsType.UnsignedInt, 0);
			
			//base.Use(sender);
		}
		protected internal override void OnDelete()
		{
			Image = null;

			base.OnDelete();
		}
	}
}
