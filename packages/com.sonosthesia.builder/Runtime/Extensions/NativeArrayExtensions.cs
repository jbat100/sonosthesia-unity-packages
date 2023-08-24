using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Sonosthesia.Builder
{
    public static class NativeArrayExtensions
    {
        public static unsafe void ClearArray<T>(this NativeArray<T> toClear, int length) where T : struct
        {
            UnsafeUtility.MemClear(
                NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(toClear),
                UnsafeUtility.SizeOf<T>() * length);
        }
    }
}