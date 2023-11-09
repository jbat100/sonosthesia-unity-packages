using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class SpawnSizeAdaptor : MapAdaptor<SpawnPayload, float>
    {
        protected override float Map(SpawnPayload source) => source.Size;
    }
}