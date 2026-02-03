using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Common.Extensions {
    public static class KeyValuePairExtensions {
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value) {
            key = tuple.Key;
            value = tuple.Value;
        }

        public static KeyValuePair<T1, T2> FirstOrRaise<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> enumerable) {
            KeyValuePair<T1, T2> res = enumerable.FirstOrDefault();
            if (res.Equals(default(KeyValuePair<T1, T2>))) {
                throw new NullReferenceException();
            }
            else {
                return res;
            }
        }
    }
}