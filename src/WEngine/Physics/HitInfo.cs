namespace WEngine
{
    public struct HitInfo
    {
        public Vector3D Position { get; }

        public Vector3D Normal { get; }

        public double Distance { get; }

        internal HitInfo(Vector3D position, Vector3D normal, double distance)
        {
            this.Position = position;
            this.Normal = normal;
            this.Distance = distance;
        }
    }
}
