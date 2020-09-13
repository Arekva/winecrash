namespace WEngine.GUI
{
    public interface IRatioKeeper
    {
        public bool KeepRatio { get; set; }

        public double Ratio { get; }

        public double GlobalRatio { get; }

        public double SizeX {get;}
        public double SizeY {get;}
    }
}
