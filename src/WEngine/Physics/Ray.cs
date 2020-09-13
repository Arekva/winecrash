namespace WEngine
{
    public struct Ray
    {
        public Vector3D Origin { get; }
        public Vector3D Direction { get; }

        public Ray(Vector3D origin, Vector3D dir)
        {
            this.Origin = origin;
            this.Direction = dir.Normalized;
        }
    }
}
