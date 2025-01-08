using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public static class RandomHelper
    {
        public static int WeightedRandom(this RandomGenerator rng, IEnumerable<int> weights)
        {
            var count = weights.Count();
            if (count <= 0)
                return -1;
            int totalWeight = weights.Sum();
            int value = rng.Next(0, totalWeight);
            for (int i = 0; i < count; i++)
            {
                value -= weights.ElementAt(i);
                if (value <= 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int WeightedRandom(this RandomGenerator rng, IEnumerable<float> weights)
        {
            var count = weights.Count();
            if (count <= 0)
                return -1;
            float totalWeight = weights.Sum();
            float value = rng.Next(0, totalWeight);
            for (int i = 0; i < count; i++)
            {
                value -= weights.ElementAt(i);
                if (value <= 0)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
