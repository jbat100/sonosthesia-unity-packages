using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class Float3ToVector3Fader : Fader<Vector3>
    {
        [SerializeField] private Fader<float3> _source;

        public override Vector3 Fade(float fade)
        {
            return _source.Fade(fade);
        }
    }
}