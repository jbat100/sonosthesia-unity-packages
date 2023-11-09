using UnityEngine;
using Sonosthesia.Flow;

namespace Sonosthesia.Spawn
{
    public class SpawnColorAdaptor : MapAdaptor<SpawnPayload, Color>
    {
        protected override Color Map(SpawnPayload source) => source.Color;
    }
}