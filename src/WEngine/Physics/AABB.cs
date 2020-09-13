namespace WEngine
{
    internal struct AABB
    {
        public Vector3D Position { get; }

        public Vector3D Extents { get; }

        public AABB(Vector3D position, Vector3D extents)
        {
            this.Position = position;
            this.Extents = extents;
        }
    }
}
