using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "FloatTouchDynamicExtractor", menuName = "Sonosthesia/Touch/FloatTouchDynamicExtractor")]
    public class FloatTouchDynamicExtractor : DynamicExtractor<TouchEvent, float>
    {
        [SerializeField] private FloatTouchDynamicExtractorSettings _settings;

        public override IDynamicExtractorSession<TouchEvent, float> MakeSession() => _settings.MakeSession();
    }
}