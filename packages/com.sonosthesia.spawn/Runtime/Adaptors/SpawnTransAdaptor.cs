using Sonosthesia.Flow;
using Unity.Mathematics;

namespace Sonosthesia.Spawn
{
    public class SpawnTransAdaptor : SimpleAdaptor<SpawnPayload, RigidTransform>
    {
        protected override RigidTransform Map(SpawnPayload source) => source.Trans;
    }
}