namespace WEngine
{
    public struct Hit
    {
        public ICollider Collider { get; }
        public Vector3D Position;

        public Vector3D Normal;

        public Vector3D Delta;
        public double Time;
        public bool HasHit;

        public Hit(ICollider collider)
        {
            this.Collider = collider;
            this.Position = Vector3D.Zero;
            this.Normal = Vector3D.Zero;
            this.Delta = Vector3D.Zero;
            this.Time = 0.0D;
            this.HasHit = true;
        }
    }
}
