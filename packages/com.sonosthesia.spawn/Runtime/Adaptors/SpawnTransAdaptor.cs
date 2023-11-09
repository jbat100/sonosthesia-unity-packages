using Unity.Mathematics;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class SpawnTransAdaptor : MapAdaptor<SpawnPayload, RigidTransform>
    {
        protected override RigidTransform Map(SpawnPayload source) => source.Trans;
    }
}