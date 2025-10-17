using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "FloatPointerStaticExtractor", menuName = "Sonosthesia/Pointer/FloatPointerStaticExtractor")]
    public class FloatPointerStaticExtractor : StaticExtractor<PointerEvent, float>
    {
        [SerializeField] private FloatPointerStaticExtractorSettings _settings;
        
        public override bool Extract(PointerEvent e, out float value) => _settings.Extract(e, out value);
    }
}