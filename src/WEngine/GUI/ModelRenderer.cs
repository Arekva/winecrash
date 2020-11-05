using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WEngine.GUI
{
    public class ModelRenderer : GUIRenderer
    {
		public Model Model { get; set; }

		internal override void PrepareForRender()
		{
			_RenderPosition = this.Model.GlobalPosition;
			_RenderRotation = this.Model.WObject.Rotation;
			_RenderScale = this.Model.GlobalScale;
			
			base.PrepareForRender();
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

                    this.Material.Use();

                    SetGLProperties();

                    DrawModel();
                }
            }
        }

		protected internal override void OnDelete()
		{
			Model = null;

			base.OnDelete();
		}
	}
}
