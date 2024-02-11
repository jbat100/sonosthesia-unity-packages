using System;
using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    public static class DictionaryExtensions
    {
        public static void Import<TKey, TValue>(this IDictionary<TKey, TValue> destination, 
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

        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, 
            TKey key, Func<TValue> creator)
        {
            if (!dictionary.TryGetValue(key, out TValue result))
            {
                result = creator();
                dictionary[key] = result;
            }

            return result;
        }

        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key) where TValue : new()
        {
            return dictionary.Ensure(key, () => new TValue());
        }
    }
}