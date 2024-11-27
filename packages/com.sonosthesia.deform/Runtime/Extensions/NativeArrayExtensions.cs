using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Sonosthesia.Deform
{
    public static class NativeArrayExtensions
    {
        public static unsafe void ClearArray<T>(this NativeArray<T> toClear) where T : struct
        {
            toClear.ClearArray(toClear.Length);
        }
        
        public static unsafe void ClearArray<T>(this NativeArray<T> toClear, int length) where T : struct
        {
            UnsafeUtility.MemClear(
                NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(toClear),
                (long)UnsafeUtility.SizeOf<T>() *  (long)length);
        }
    }
}