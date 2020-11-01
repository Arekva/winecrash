namespace WEngine
{
    public class FreeBoxCollider : AABBCollider
    {
        public override AABB AABB
        {
            get
            {
                return new AABB(Center, Extents);
            }
            set
            {
                Center = value.Position;
                Extents = value.Extents;
            }
        }

        public override Vector3D Extents { get; set; }
        public override Vector3D Center { get; set; }
    }
}