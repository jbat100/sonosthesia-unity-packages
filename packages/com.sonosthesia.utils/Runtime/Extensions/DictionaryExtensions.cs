using System;
using System.Collections.Generic;
using UniRx;

namespace Sonosthesia.Utils
{
    public static class DictionaryExtensions
    {
        public static void Import<TKey, TValue>(this Dictionary<TKey, TValue> destination, 
            IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            foreach (var kvp in source)
            {
                destination[kvp.Key] = kvp.Value;
            }
        }
    }
}