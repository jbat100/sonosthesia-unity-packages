using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public enum SafeIndex
    {
        None,
        Clamp,
        Loop
    }

    public static class SafeIndexListExtensions
    {
        public static bool TryGetIndex<T>(this IList<T> list, float index, SafeIndex safeIndex, out T result)
        {
            return list.TryGetIndex(Mathf.RoundToInt(index), safeIndex, out result);
        }
        
        public static bool TryGetIndex<T>(this IList<T> list, int index, SafeIndex safeIndex, out T result)
        {
            if (index >= 0 && index < list.Count)
            {
                result = list[index];
                return true;
            }

            switch (safeIndex)
            {
                case SafeIndex.Clamp:
                    result = list[Mathf.Clamp(index, 0, list.Count - 1)];
                    return true;
                case SafeIndex.Loop:
                {
                    result = list[index % list.Count];
                    return true;
                };
                default:
                {
                    result = default;
                    return false;
                };
            }
        }
    }
}