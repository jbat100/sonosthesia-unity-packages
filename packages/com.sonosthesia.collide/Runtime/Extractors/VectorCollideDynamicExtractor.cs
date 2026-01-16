using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Collide
{
    [CreateAssetMenu(fileName = "VectorCollideDynamicExtractor", menuName = "Sonosthesia/Collide/VectorCollideDynamicExtractor")]
    public class VectorCollideDynamicExtractor : SettingsDynamicExtractor<CollideEvent, Vector3, VectorCollideDynamicExtractorSettings>
    {
        
    }
}