namespace WEngine
{
    public static class Physics
    {
        public static Vector3D Gravity { get; set; } = Vector3D.Down * 9.81D;

        private static double _FixedRate = 1D / 60D;
        public static double FixedRate
        {
            get
            {
                return _FixedRate;
            }

            set
            {
                _FixedRate = value;
            }
        }
        
        //public static 
    }
}
