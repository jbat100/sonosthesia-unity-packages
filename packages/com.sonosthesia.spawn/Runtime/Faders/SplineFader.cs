using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Spawn
{
    public class SplineFader : Fader<float3>
    {
        [SerializeField] private SplineVector _vector;
        
        [SerializeField] private Spline _spline;
        
        public override float3 Fade(float fade)
        {
            return _spline.Vector(fade, _vector);
        }
    }
}