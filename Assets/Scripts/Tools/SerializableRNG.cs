namespace Tools
{
    public class SerializableRNG
    {
        public SerializableRNG(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        internal SerializableRNG(XORShift128 generator) : this((int)generator.x, (int)generator.y, (int)generator.z, (int)generator.w)
        {
        }
        public int x;
        public int y;
        public int z;
        public int w;
    }
}
