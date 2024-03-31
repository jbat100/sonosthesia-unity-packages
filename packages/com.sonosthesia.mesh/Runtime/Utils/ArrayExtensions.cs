using Unity.Collections;

namespace Sonosthesia.Mesh
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Recreates an persistent array with uninitialized memory if current is not create or different length
        /// Useful when we expect contents to be updated often with length which is updated rarely
        /// </summary>
        /// <param name="array"></param>
        /// <param name="capacity"></param>
        /// <typeparam name="T"></typeparam>
        public static void TryReusePersistentArray<T>(this ref NativeArray<T> array, int capacity) where T : struct
        {
            if (array.IsCreated && array.Length == capacity)
            {
                return;
            }
            if (array.IsCreated)
            {
                array.Dispose();
            }
            array = new NativeArray<T>(capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);;
        }
    }
}