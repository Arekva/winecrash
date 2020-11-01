namespace WEngine
{
    public struct Ray : ICollider
    {
        public Vector3D Origin { get; }
        public Vector3D Direction { get; }
        
        public double Length { get; set; }

        public Ray(Vector3D origin, Vector3D dir, double length)
        {
            this.Origin = origin;
            this.Direction = dir.Normalized;
            this.Length = length;
        }
    }
}
