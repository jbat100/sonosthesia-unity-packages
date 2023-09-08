using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public interface IDynamicProcessor<T> where T : struct
    {
        T Process(T input, float time);

        void Reset();
    }

    public interface IDynamicProcessorSettings
    {
        bool Bypass { get; }
    }

    // settings should be reference type as the idea is that they are inspector settings which can change on the fly
    
    public abstract class DynamicProcessor<T, TSettings> : IDynamicProcessor<T> 
        where T : struct where TSettings : class, IDynamicProcessorSettings
    {
        private readonly TSettings _settings;

        public DynamicProcessor(TSettings settings)
        {
            _settings = settings;
        }

        public T Process(T input, float time)
        {
            return _settings.Bypass ? input : Process(_settings, input, time);
        }

        protected abstract T Process(TSettings settings, T input, float time);
        
        public virtual void Reset() { }
    }

    [Serializable]
    public class DynamicProcessorSettings : IDynamicProcessorSettings
    {
        [SerializeField] private bool _bypass;
        public bool Bypass => _bypass;
    }
}