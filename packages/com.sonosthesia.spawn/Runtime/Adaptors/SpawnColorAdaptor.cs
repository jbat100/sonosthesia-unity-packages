using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnColorAdaptor : MapAdaptor<SpawnPayload, Color>
    {
        protected override Color Map(SpawnPayload source) => source.Color;
    }
}