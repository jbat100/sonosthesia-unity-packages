using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TrackedTouchEnvelopeConfiguration", 
        menuName = "Sonosthesia/Touch/TrackedTouchEnvelopeConfiguration")]

    public class TrackedTouchEnvelopeConfiguration : ScriptableObject
    {
        [SerializeField] private TrackedTouchEnvelopeSettings _settings;
        public TrackedTouchEnvelopeSettings Settings => _settings;
    }
}