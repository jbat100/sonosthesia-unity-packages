using Sonosthesia.Mesh;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Sonosthesia.Deform
{
    
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct TubeJob<G, S> : IJobFor
        where G : struct, ITubeGenerator
        where S : struct, IMeshStreams
    {
        private G _generator;
        
        [WriteOnly] private S _streams;
        
        public void Execute(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}