using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnVector3PositionAdaptor : SimpleAdaptor<SpawnPayload, Vector3>
    {
        protected override Vector3 Map(SpawnPayload source)
        {
            return source.Trans.pos;
        }
    }
}