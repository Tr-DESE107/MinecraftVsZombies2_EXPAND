using System;

namespace Tools
{
    public class RandomGenerator
    {
        public RandomGenerator(int seed)
        {
            generator = new Random(seed);
            Seed = seed;
        }
        public RandomGenerator(int seed, int times)
        {
            generator = new Random(seed);
            Seed = seed;
            Times = times;
            for (int i = 0; i < times; i++)
            {
                generator.Next();
            }
        }


        public int Next()
        {
            var value = generator.Next();
            Times++;
            return value;
        }
        public int Next(int min, int max)
        {
            var value = generator.Next(min, max);
            Times++;
            return value;
        }
        public float Next(float min, float max)
        {
            return min + (float)Next(0, int.MaxValue) / int.MaxValue * (max - min);
        }
        public int WeightedRandom(float[] weights)
        {
            float totalWeight = 0;
            foreach (int weight in weights)
            {
                totalWeight += weight;
            }
            float value = Next(0, totalWeight);
            for (int i = 0; i < weights.Length; i++)
            {
                value -= weights[i];
                if (value <= 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public SerializableRNG Serialize()
        {
            return new SerializableRNG()
            {
                times = Times,
                seed = Seed
            };
        }
        public static RandomGenerator Deserialize(SerializableRNG seri)
        {
            return new RandomGenerator(seri.seed, seri.times);
        }
        public int Times { get; set; }
        public int Seed { get; set; }
        private Random generator;
    }
}
