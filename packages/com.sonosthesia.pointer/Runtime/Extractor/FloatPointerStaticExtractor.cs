using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    [CreateAssetMenu(fileName = "FloatPointerStaticExtractor", menuName = "Sonosthesia/Pointer/FloatPointerStaticExtractor")]
    public class FloatPointerStaticExtractor 
        : SettingsStaticExtractor<PointerEvent, float, FloatPointerStaticExtractorSettings>
    {

    }
}