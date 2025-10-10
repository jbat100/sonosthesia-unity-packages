using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "FloatTouchStaticExtractor", menuName = "Sonosthesia/Touch/FloatTouchStaticExtractor")]
    public class FloatTouchStaticExtractor : StaticExtractor<TouchEvent, float>
    {
        [SerializeField] private FloatTouchStaticExtractorSettings _settings;

        public override bool Extract(TouchEvent e, out float value) => _settings.Extract(e, out value);
    }
}