using UnityEngine;

namespace Sonosthesia.Touch
{
    public class VectorTouchExtractor : TouchExtractor<Vector3>
    {
        [SerializeField] private VectorTouchExtractorSettings _settings;

        public override ITouchExtractorSession<Vector3> MakeSession() => _settings.MakeSession();
    }
}