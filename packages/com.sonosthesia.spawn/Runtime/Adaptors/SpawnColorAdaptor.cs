using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnColorAdaptor : SimpleAdaptor<SpawnPayload, Color>
    {
        protected override Color Map(SpawnPayload source) => source.Color;
    }
}