using UnityEngine;
using FMODUnity;
using Sonosthesia.Touch;

namespace Sonosthesia.FMODInteraction
{
    [CreateAssetMenu(fileName = "TouchFMODEmitterAffordance", menuName = "Sonosthesia/Touch/TouchFMODEmitterAffordance")]
    public class TouchFMODEmitterConfiguration : ScriptableObject
    {
        [SerializeField] private StudioEventEmitter _emitterPrefab;
        public StudioEventEmitter EmitterPrefab => _emitterPrefab;
        
        [SerializeField] private DynamicTrackingSettings _positionTracking;
        public DynamicTrackingSettings PositionTracking => _positionTracking;

        [SerializeField] private TouchEnvelopeSettings _volume;
        public TouchEnvelopeSettings Volume => _volume;
        
        [SerializeField] private TouchEnvelopeSettings _excitation;
        public TouchEnvelopeSettings Excitation => _excitation;
        
        [SerializeField] private TouchEnvelopeSettings _body;
        public TouchEnvelopeSettings Body => _body;
    }
}