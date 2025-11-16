using Sonosthesia.Processing;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class Trigger : Signal<float>
    {
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private DynamicProcessorFactory<float> _postProcessorFactory;

        [SerializeField] private FloatProcessorSettings _postProcessor;
        
        private IDynamicProcessor<float> _dynamicPostProcessor;

        public TriggerImplementation TriggerImplementation { get; private set; }
        
        private void SetupState()
        {
            TriggerImplementation?.Clear();
            TriggerImplementation = new TriggerImplementation(_accumulationMode);
            _dynamicPostProcessor = _postProcessorFactory ? _postProcessorFactory.Make() : null;
        }

        protected virtual void OnValidate() => SetupState();

        protected virtual void OnEnable() => SetupState();
        
        protected virtual void Update()
        {
            float result = TriggerImplementation.Evaluate();
            if (_dynamicPostProcessor != null)
            {
                result = _dynamicPostProcessor.Process(result, Time.time);
            }
            result = _postProcessor.Process(result);
            Broadcast(result);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TriggerImplementation?.Clear();
            TriggerImplementation = null;
        }
    }
}