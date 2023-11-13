using Sonosthesia.Noise;
using Unity.Collections;
using Unity.Jobs;

namespace Sonosthesia.Deform
{
    public struct SumArrayJob<T> : IJobFor where T : struct, ISummable<T>
    {
        public NativeArray<T> source;
        public NativeArray<T> target;

        public void Execute(int index)
        {
            target[index] = source[index].Sum(target[index]);
        }
    }
}