using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "FloatPointerExtractor", menuName = "Sonosthesia/Pointer/FloatPointerExtractor")]
    public class FloatPointerExtractor : DynamicExtractor<PointerEvent, float>
    {
        [SerializeField] private FloatPointerExtractorSettings _settings;

        public override IExtractorSession<PointerEvent, float> MakeSession() => _settings.MakeSession();
    }
    
}