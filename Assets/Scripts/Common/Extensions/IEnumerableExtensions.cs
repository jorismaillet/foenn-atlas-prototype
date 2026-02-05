using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            foreach (T element in enumerable)
            {
                action.Invoke(element);
            }
        }

        public static bool Empty<T>(this IEnumerable<T> enumerable) where T : class
        {
            return !enumerable.Any();
        }

        public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, T element) where T : class
        {
            return enumerable.Where(e => !e.Equals(element));
        }

        public static IEnumerable<T> With<T>(this IEnumerable<T> enumerable, T element) where T : class
        {
            foreach (T t in enumerable.Without(element))
            {
                yield return t;
            }
            yield return element;
            yield break;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.Where(e => e != null);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> Flatten<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable, Func<TValue> func)
        {

            return enumerable;
        }

        public static IEnumerable<TDest> Filter<TDest>(this IEnumerable<object> enumerable)
        {
            foreach (object el in enumerable)
            {
                if (el is TDest dest)
                {
                    yield return dest;
                }
            }
            yield break;
        }

        public static T FirstOrNull<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : class
        {
            T res = enumerable.FirstOrDefault(predicate);
            if (res == null || res.Equals(default(T)))
            {
                return null;
            }
            else
            {
                return res;
            }
        }

        public static T FirstOrNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            T res = enumerable.FirstOrDefault();
            if (res == null || res.Equals(default(T)))
            {
                return null;
            }
            else
            {
                return res;
            }
        }

        public static void RemoveFirst<T>(this List<T> enumerable, Predicate<T> match) where T : class
        {
            int index = enumerable.FindIndex(match);
            if (index > -1)
            {
                enumerable.RemoveAt(index);
            }
        }

        public static void Replace<T>(this List<T> enumerable, T newElement, Predicate<T> match) where T : class
        {
            int index = enumerable.FindIndex(match);
            if (index > -1)
            {
                enumerable.RemoveAt(index);
                enumerable.Insert(index, newElement);
            }
        }

        public static void Replace<T>(this List<T> enumerable, IEnumerable<T> newElements) where T : class
        {
            enumerable.Clear();
            enumerable.AddRange(newElements);
        }
    }
}