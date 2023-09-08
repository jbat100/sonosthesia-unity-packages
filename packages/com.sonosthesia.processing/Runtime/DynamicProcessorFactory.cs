using UnityEngine;

namespace Sonosthesia.Processing
{
    public abstract class DynamicProcessorFactory<T> : ScriptableObject where T : struct
    {
        public abstract IDynamicProcessor<T> Make();
    }

    public abstract class ConfigurableDynamicProcessorFactory<TSettings, T> : DynamicProcessorFactory<T>
        where TSettings : class, IDynamicProcessorSettings where T : struct
    {
        [SerializeField] private TSettings _settings;

        public sealed override IDynamicProcessor<T> Make()
        {
            return Make(_settings);
        }
        
        protected abstract IDynamicProcessor<T> Make(TSettings settings);
    }
}