using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "FloatPointerDynamicExtractor", menuName = "Sonosthesia/Pointer/FloatPointerDynamicExtractor")]
    public class FloatPointerDynamicExtractor : DynamicExtractor<PointerEvent, float>
    {
        [SerializeField] private FloatPointerDynamicExtractorSettings _settings;

        public override IDynamicExtractorSession<PointerEvent, float> MakeSession() => _settings.MakeSession();
    }
    
}