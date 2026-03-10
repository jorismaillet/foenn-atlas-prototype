namespace Assets.Scripts.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ListUtil
    {
        public static List<T> IntersectAll<T>(IEnumerable<IEnumerable<T>> lists)
        {
            if (!lists.Any())
            {
                return new List<T>();
            }
            HashSet<T> hashSet = new HashSet<T>(lists.First());
            foreach (var list in lists.Skip(1))
            {
                if (!hashSet.Any() || !list.Any())
                {
                    return new List<T>();
                }
                hashSet.IntersectWith(list);
            }
            return hashSet.ToList();
        }

        public static void Replace<T>(List<T> originalList, List<T> elementsToReplace, Predicate<T> match)
        {
            elementsToReplace.ForEach(elementToReplace =>
            {
                Replace(originalList, elementToReplace, match);
            });
        }

        public static void Replace<T>(List<T> originalList, T elementToReplace, Predicate<T> match)
        {
            T result = originalList.Find(match);
            if (result == null || result.Equals(default(T)))
            {
                return;
            }
            int index = originalList.IndexOf(result);
            originalList.RemoveAt(index);
            originalList.Insert(index, elementToReplace);
        }

        public static IEnumerable<T> TakeEven<T>(this List<T> scope, int startIndex)
        {
            int maxIndex = scope.Count - 1;
            int iterations = Math.Max(startIndex + 1, maxIndex - startIndex);
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                int index = startIndex - iteration;
                if (index >= 0)
                {
                    yield return scope[index];
                }
                index = startIndex + iteration + 1;
                if (index <= maxIndex)
                {
                    yield return scope[index];
                }
            }
            yield break;
        }
    }
}
