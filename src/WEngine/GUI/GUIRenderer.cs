namespace WEngine.GUI
{
    public abstract class GUIRenderer : MeshRenderer
    {
        protected internal override void Creation()
        {
            this.UseDepth = false;
            this.UseMask = true;

            base.Creation();
        }

        internal override void PrepareForRender()
        {
            _RenderUp = this.WObject.Up;
            _RenderForward = this.WObject.Forward;
        }
    }
}
