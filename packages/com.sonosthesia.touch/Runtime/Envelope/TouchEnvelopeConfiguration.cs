using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchEnvelopeConfiguration", 
        menuName = "Sonosthesia/Touch/TouchEnvelopeConfiguration")]

    public class TouchEnvelopeConfiguration : ScriptableObject
    {
        [SerializeField] private TouchEnvelopeSettings _settings;
        public TouchEnvelopeSettings Settings => _settings;
    }
}