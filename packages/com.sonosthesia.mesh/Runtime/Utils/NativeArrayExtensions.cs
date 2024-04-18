using Unity.Collections;
using Unity.Mathematics;

namespace Sonosthesia.Mesh
{
    public static class NativeArrayExtensions
    {
        /// <summary>
        /// Recreates an persistent array with uninitialized memory if current is not created or different capacity
        /// Useful when we expect contents to be updated often with length which is updated rarely
        /// </summary>
        /// <param name="array"></param>
        /// <param name="capacity"></param>
        /// <typeparam name="T"></typeparam>
        public static bool TryReusePersistentArray<T>(this ref NativeArray<T> array, int capacity) where T : struct
        {
            if (array.IsCreated && array.Length == capacity)
            {
                return true;
            }
            if (array.IsCreated)
            {
                array.Dispose();
            }
            array = new NativeArray<T>(capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            return false;
        }

        // Note : use Reinterpret instead
        public static float4 Slice4(this NativeArray<float> array, int startIndex)
        {
            float4 result = default;
            int length = array.Length;
            for (int i = 0; i < 4; ++i)
            {
                int arrayIndex = startIndex + i;
                if (arrayIndex < length)
                {
                    result[i] = array[startIndex + i];   
                }
            }
            return result;
        }
    }
}