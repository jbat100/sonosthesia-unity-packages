using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public abstract class DynamicMeshNoiseDomainTarget<T, B> : DynamicMeshNoiseTarget<T, B> 
        where T : struct where B : struct, IBlender<T>
    {
        
    }
}