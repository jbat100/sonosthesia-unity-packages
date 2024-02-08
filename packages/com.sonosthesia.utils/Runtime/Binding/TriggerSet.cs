using System;
using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    public static class TriggerSet
    {
        // TODO : move to UI Toolkit bindings once we can use in world space XR
        
        public static void Set<T>(T updated, ref T value, Action onChanged)
        {
            Set(updated, ref value, onChanged, EqualityComparer<T>.Default);
        }
        
        public static void Set<T>(T updated, ref T value, Action onChanged, IEqualityComparer<T> comparer)
        {
            if (!comparer.Equals(updated, value))
            {
                value = updated;
                onChanged?.Invoke();
            }
        }
    }
}