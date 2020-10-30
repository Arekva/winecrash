namespace WEngine.Debugging
{
    public class BoxDebugVolume : DebugVolume
    {
        private Vector3D _Offset = Vector3D.Zero;
        public Vector3D Offset
        {
            get
            {
                return _Offset;
            }

            set
            {
                _Offset = value;
                
                Renderer?.Material.SetData("offset", value);
            }
        }
        
        private Vector3D _Extents = Vector3D.One / 2D;
        public Vector3D Extents
        {
            get
            {
                return _Extents;
            }

            set
            {
                _Extents = value;
                
                Renderer?.Material.SetData("extents", value);
            }
        }

        protected internal override void Creation()
        {
            base.Creation();

            //Renderer.Material.SetData("offset", Offset);
            Extents = _Extents;

            //Renderer.Material.SetData("extents", Extents);
        }
    }
}