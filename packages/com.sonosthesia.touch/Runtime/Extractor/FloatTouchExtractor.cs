using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "FloatTouchExtractor", menuName = "Sonosthesia/Touch/FloatTouchExtractor")]
    public class FloatTouchExtractor : TouchExtractor<float>
    {
        [SerializeField] private FloatTouchExtractorSettings _settings;

        public override ITouchExtractorSession<float> MakeSession() => _settings.MakeSession();
    }
}