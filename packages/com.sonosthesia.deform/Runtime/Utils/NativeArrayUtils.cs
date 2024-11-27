using Unity.Collections;

namespace Sonosthesia.Deform
{
    public static class NativeArrayUtils
    {
        // note : if the array was already created and with expected size, then allocator and options are ignored
        
        public static void EnsureNativeArrayLength<T>(this ref NativeArray<T> array, int length, 
            Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory) 
            where T : struct
        {
            if (array.IsCreated && array.Length == length)
            {
                return;
            }

            if (array.IsCreated)
            {
                array.Dispose();
            }
            
            array = new NativeArray<T>(length, allocator, options);
        }
    }
}