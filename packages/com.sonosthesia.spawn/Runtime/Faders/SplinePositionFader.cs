using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Spawn
{
    public class SplinePositionFader : Fader<float3>
    {
        [SerializeField] private SplineContainer _splineContainer;

        [SerializeField] private int _index;
        
        public override float3 Fade(float fade)
        {
            return _splineContainer.EvaluatePosition(_index, fade);
        }
    }
}