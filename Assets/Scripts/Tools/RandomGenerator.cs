using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Tools
{
    [Serializable]
    public class RandomGenerator
    {
        public RandomGenerator(int seed)
        {
            Seed = seed;
        }
        public RandomGenerator(int seed, int times)
        {
            Seed = seed;
            Times = times;
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
        [BsonElement("times")]
        public int Times { get; private set; }
        [BsonElement("seed")]
        public int Seed { get; private set; }
        [BsonIgnore]
        private Random generator
        {
            get
            {
                if (_generator == null)
                {
                    _generator = new Random(Seed);
                    for (int i = 0; i < Times; i++)
                    {
                        generator.Next();
                    }
                }
                return _generator;
            }
        }
        private Random _generator;
    }
}
