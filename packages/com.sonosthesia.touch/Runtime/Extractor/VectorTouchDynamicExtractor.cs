using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "VectorTouchDynamicExtractor", menuName = "Sonosthesia/Touch/VectorTouchDynamicExtractor")]
    public class VectorTouchDynamicExtractor : DynamicExtractor<TouchEvent, Vector3>
    {
        [SerializeField] private VectorTouchDynamicExtractorSettings _settings;

        public override IDynamicExtractorSession<TouchEvent, Vector3> MakeSession() => _settings.MakeSession();
    }
}