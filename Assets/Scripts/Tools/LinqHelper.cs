using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public static class LinqHelper
    {
        public static T Random<T>(this IEnumerable<T> list, RandomGenerator rng)
        {
            var index = rng.Next(0, list.Count());
            return list.ElementAt(index);
        }
        public static IEnumerable<T> RandomTake<T>(this IEnumerable<T> list, int count, RandomGenerator rng)
        {
            List<T> results = new List<T>();
            List<T> pool = new List<T>(list);
            for (int i = 0; i < count; i++)
            {
                var poolCount = pool.Count;
                if (poolCount <= 0)
                    break;
                var index = rng.Next(0, poolCount);
                var element = pool[index];
                results.Add(element);
                pool.Remove(element);
            }
            return results;
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list, RandomGenerator rng)
        {
            return list.RandomTake(list.Count(), rng);
        }
    }
}
