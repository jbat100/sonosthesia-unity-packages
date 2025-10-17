using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "PointerEnvelopeConfiguration", menuName = "Sonosthesia/Pointer/PointerEnvelopeConfiguration")]
    public class PointerEnvelopeConfiguration: ScriptableObject
    {
        [SerializeField] private PointerEnvelopeSettings _settings;

        public PointerEnvelopeSettings Settings => _settings;
    }
}