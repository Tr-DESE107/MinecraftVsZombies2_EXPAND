using System.Collections.Generic;

namespace Tools
{
    public static class RandomHelper
    {
        public static int WeightedRandom(this RandomGenerator rng, IList<int> weights)
        {
            int totalWeight = 0;
            foreach (int weight in weights)
            {
                totalWeight += weight;
            }
            int value = rng.Next(0, totalWeight);
            for (int i = 0; i < weights.Count; i++)
            {
                value -= weights[i];
                if (value <= 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int WeightedRandom(this RandomGenerator rng, IList<float> weights)
        {
            float totalWeight = 0;
            foreach (int weight in weights)
            {
                totalWeight += weight;
            }
            float value = rng.Next(0, totalWeight);
            for (int i = 0; i < weights.Count; i++)
            {
                value -= weights[i];
                if (value <= 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
