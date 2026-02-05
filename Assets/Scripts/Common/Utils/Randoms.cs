using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Common.Utils
{
    public static class Randoms
    {

        //Function to get random number
        private static readonly Random random = new Random();

        private static readonly object syncLock = new object();
        public static int Get(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }

        public static int Get(Tuple<int, int> range)
        {
            return Get(range.Item1, range.Item2);
        }

        public static float Get(Tuple<float, float> range)
        {
            return Float(range.Item1, range.Item2);
        }

        public static int Get(int max)
        {
            return Get(0, max);
        }

        public static int Get()
        {
            lock (syncLock)
            {
                return random.Next();
            }
        }

        public static double Double()
        {
            lock (syncLock)
            {
                return random.NextDouble();
            }
        }

        public static bool Proc(float percentage)
        {
            lock (syncLock)
            {
                return Float(0, 100) <= percentage;
            }
        }

        public static float Float(float min, float max)
        {
            lock (syncLock)
            {
                return Convert.ToSingle(random.NextDouble() * (max - min) + min);
            }
        }

        public static IEnumerable<T> RandomTake<T>(this IEnumerable<T> list, int min, int max)
        {
            return list.OrderBy(x => Get()).Take(Get(min, max + 1));
        }

        public static IEnumerable<T> RandomTake<T>(this IEnumerable<T> list, int max)
        {
            return list.RandomTake(max, max);
        }
    }
}