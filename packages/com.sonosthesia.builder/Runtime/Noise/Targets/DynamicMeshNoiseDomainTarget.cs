using Sonosthesia.Signal;

namespace Sonosthesia.Builder
{
    public abstract class DynamicMeshNoiseDomainTarget<T, B> : DynamicMeshNoiseTarget<T, B> 
        where T : struct where B : struct, IBlender<T>
    {
        
    }
}