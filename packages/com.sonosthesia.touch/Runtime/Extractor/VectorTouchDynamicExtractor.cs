using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "VectorTouchDynamicExtractor", menuName = "Sonosthesia/Touch/VectorTouchDynamicExtractor")]
    public class VectorTouchDynamicExtractor : SettingsDynamicExtractor<TouchEvent, Vector3, VectorTouchDynamicExtractorSettings>
    {
        
    }
}