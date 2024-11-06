using UnityEngine;

namespace Sonosthesia.Touch
{
    public class TestTouchActorModulator : TouchActorModulator
    {
        [SerializeField] [Range(0, 1)] private float _curl;
        [SerializeField] [Range(0, 1)] private float _tension;
        [SerializeField] [Range(0, 1)] private float _pinch;
        
        public override float Select(TouchActorModulationType modulationType)
        {
            return modulationType switch
            {
                TouchActorModulationType.Curl => _curl,
                TouchActorModulationType.Tension => _tension,
                TouchActorModulationType.Pinch => _pinch,
                _ => 0
            };
        }
    }
}