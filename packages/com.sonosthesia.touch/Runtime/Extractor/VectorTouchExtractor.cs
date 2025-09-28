using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class VectorTouchExtractor : Extractor<TouchEvent, Vector3>
    {
        [SerializeField] private VectorTouchExtractorSettings _settings;

        public override IExtractorSession<TouchEvent, Vector3> MakeSession() => _settings.MakeSession();
    }
}