using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine.GUI
{
	public sealed class ImageRenderer : GUIRenderer
	{
		public Image Image { get; set; }


		private static Mesh _Panel;

		public override Mesh Mesh
		{
			get
			{
				return _Panel;
			}
			set
			{
				Debug.LogWarning("Image's mesh is read-only.");
			}
		}

		internal override void PrepareForRender()
		{
			_renderPosition = this.Image.GlobalPosition;
			_renderRotation = this.Image.WObject.Rotation;
			_renderScale = this.Image.GlobalScale;

			base.PrepareForRender();
		}

		static ImageRenderer()
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

			_Panel.Apply(true);
		}

		internal override void Use(Camera sender)
		{
			if (MeshLocker == null || MaterialLocker == null) return;

			lock (MaterialLocker)
			{
				lock (MeshLocker)
				{
					if (CheckValidity(sender)) return;
                    
					BindBuffers();
					ComputeMatricesGPU(sender);
                    
					this.Material.Use(sender);

					SetGLProperties();

					DrawModel();
				}
			}
		}
		protected internal override void OnDelete()
		{
			Image = null;

			base.OnDelete();
		}
	}
}
