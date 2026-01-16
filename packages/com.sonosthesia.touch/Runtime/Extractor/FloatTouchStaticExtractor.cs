using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "FloatTouchStaticExtractor", menuName = "Sonosthesia/Touch/FloatTouchStaticExtractor")]
    public class FloatTouchStaticExtractor 
        : SettingsStaticExtractor<TouchEvent, float, FloatTouchStaticExtractorSettings>
    {

    }
}