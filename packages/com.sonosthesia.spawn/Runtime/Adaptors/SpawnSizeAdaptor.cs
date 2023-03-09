using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class SpawnSizeAdaptor : SimpleAdaptor<SpawnPayload, float>
    {
        protected override float Map(SpawnPayload source) => source.Size;
    }
}