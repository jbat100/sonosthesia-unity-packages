using UnityEngine;

namespace Sonosthesia.Extractor
{
    public abstract class DynamicExtractor<TEvent, TValue> : ScriptableObject, IDynamicExtractor<TEvent, TValue>
        where TValue : struct
    {
        public abstract IDynamicExtractorSession<TEvent, TValue> MakeSession();
    }

    // wrap a serializable POCO IDynamicExtractor in a scriptable object
    
    public class SettingsDynamicExtractor<TEvent, TValue, TSettings> : ScriptableObject, IDynamicExtractor<TEvent, TValue>
        where TValue : struct
        where TSettings : IDynamicExtractor<TEvent, TValue>
    {
        [SerializeField] private TSettings _settings;

        public IDynamicExtractorSession<TEvent, TValue> MakeSession() => _settings.MakeSession();
    }
}