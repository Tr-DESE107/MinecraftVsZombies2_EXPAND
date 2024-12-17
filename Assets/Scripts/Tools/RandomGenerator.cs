using System;

namespace Tools
{
    [Serializable]
    public class RandomGenerator
    {
        public RandomGenerator(int seed)
        {
            if (seed == 0)
                seed = -1;
            generator = new XORShift128();
            generator.InitSeed(seed);
        }
        private RandomGenerator(int x, int y, int z, int w)
        {
            generator = new XORShift128();
            generator.InitState((uint)x, (uint)y, (uint)z, (uint)w);
        }


        public int Next()
        {
            return generator.NextInt();
        }
        public float NextFloat()
        {
            return generator.NextFloat();
        }
        public int Next(int max)
        {
            return Next(0, max);
        }
        public float Next(float max)
        {
            return Next(0, max);
        }
        public int Next(int min, int max)
        {
            return generator.NextIntRange(min, max);
        }
        public float Next(float min, float max)
        {
            return generator.NextFloatRange(min, max);
        }
        public SerializableRNG ToSerializable()
        {
            return new SerializableRNG(generator);
        }
        public static RandomGenerator FromSerializable(SerializableRNG seri)
        {
            return new RandomGenerator(seri.x, seri.y, seri.z, seri.w);
        }
        private XORShift128 generator;
    }
}
