using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Spawn
{
    public class SplineRotationFader : Fader<quaternion>
    {
        [SerializeField] private SplineVector _forward;
        
        [SerializeField] private SplineVector _up; 
        
        [SerializeField] private Spline _spline;
        
        public override quaternion Fade(float fade)
        {
            return _spline.Rotation(fade, _forward, _up);
        }
    }
}