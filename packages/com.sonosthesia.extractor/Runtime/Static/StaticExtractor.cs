using UnityEngine;

namespace Sonosthesia.Extractor
{
    public abstract class StaticExtractor<TEvent, TValue> : ScriptableObject, IStaticExtractor<TEvent, TValue>
       where TValue : struct
    {
        public abstract bool Extract(TEvent e, out TValue value);
    }
    
    // wrap a serializable POCO IStaticExtractor in a scriptable object

    public class SettingsStaticExtractor<TEvent, TValue, TSettings> : ScriptableObject, IStaticExtractor<TEvent, TValue>
        where TValue : struct
        where TSettings : IStaticExtractor<TEvent, TValue>
    {
        [SerializeField] private TSettings _settings;

        public bool Extract(TEvent e, out TValue value) => _settings.Extract(e, out value);
    }
}