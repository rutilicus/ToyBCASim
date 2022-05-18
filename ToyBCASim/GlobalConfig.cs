namespace ToyBCASim
{
    internal class GlobalConfig
    {
        private static GlobalConfig _instance = new();
        public int Palette = 0;
        public int GridSize = 32;
        public int Delay = 100;

        public static GlobalConfig Instance
        {
            get { return _instance; }
        }
    }
}
