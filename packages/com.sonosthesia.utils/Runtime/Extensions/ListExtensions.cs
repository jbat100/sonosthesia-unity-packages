using System.Collections.Generic;

namespace Sonosthesia.Utils
{
    public static class ListExtensions
    {
        // useful for easy static list reuse in the Unity main thread code
        // returns list for fluency
        
        public static List<T> Import<T>(this List<T> list, IEnumerable<T> enumerable)
        {
            list.Clear();
            list.AddRange(enumerable);
            return list;
        }
    }
}