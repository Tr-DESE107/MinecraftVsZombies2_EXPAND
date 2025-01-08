using System;
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
        public static T WeightedRandom<T>(this IEnumerable<T> list, Func<T, int> weightGetter, RandomGenerator rng)
        {
            var count = list.Count();
            if (count <= 0)
                throw new ArgumentException("The list to get weighted random element is empty.");
            var weights = list.Select(i => weightGetter(i));
            int totalWeight = weights.Sum();
            int value = rng.Next(0, totalWeight);
            for (int i = 0; i < count; i++)
            {
                value -= weights.ElementAt(i);
                if (value <= 0)
                {
                    return list.ElementAt(i);
                }
            }
            return default;
        }
        public static IEnumerable<T> WeightedRandomTake<T>(this IEnumerable<T> list, IList<int> weights, int count, RandomGenerator rng)
        {
            List<T> results = new List<T>();

            List<T> pool = new List<T>(list);
            List<int> weightPool = new List<int>(weights);
            for (int i = 0; i < count; i++)
            {
                if (pool.Count <= 0 || weights.Count <= 0)
                    break;
                var index = rng.WeightedRandom(weightPool);
                var element = pool[index];

                results.Add(element);

                pool.RemoveAt(index);
                weightPool.RemoveAt(index);
            }
            return results;
        }
        public static IEnumerable<T> WeightedRandomTake<T>(this IEnumerable<T> list, IList<float> weights, int count, RandomGenerator rng)
        {
            List<T> results = new List<T>();

            List<T> pool = new List<T>(list);
            List<float> weightPool = new List<float>(weights);
            for (int i = 0; i < count; i++)
            {
                if (pool.Count <= 0 || weights.Count <= 0)
                    break;
                var index = rng.WeightedRandom(weightPool);
                var element = pool[index];

                results.Add(element);

                pool.RemoveAt(index);
                weightPool.RemoveAt(index);
            }
            return results;
        }
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list, RandomGenerator rng)
        {
            return list.RandomTake(list.Count(), rng);
        }
        public static IEnumerable<T> TakeWhileLast<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            return list.Reverse().TakeWhile(predicate).Reverse();
        }
    }
}
