using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "FloatTouchExtractor", menuName = "Sonosthesia/Touch/FloatTouchExtractor")]
    public class FloatTouchExtractor : Extractor<TouchEvent, float>
    {
        [SerializeField] private FloatTouchExtractorSettings _settings;

        public override IExtractorSession<TouchEvent, float> MakeSession() => _settings.MakeSession();
    }
}