using Sonosthesia.Processing;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class Trigger : Signal<float>
    {
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private DynamicProcessorFactory<float> _postProcessorFactory;

        [SerializeField] private FloatProcessor _postProcessor;
        
        private IDynamicProcessor<float> _dynamicPostProcessor;

        public TriggerController TriggerController { get; private set; }
        
        private void SetupState()
        {
            TriggerController?.Clear();
            TriggerController = new TriggerController(_accumulationMode);
            _dynamicPostProcessor = _postProcessorFactory ? _postProcessorFactory.Make() : null;
        }

        protected virtual void OnValidate() => SetupState();

        protected virtual void OnEnable() => SetupState();
        
        protected virtual void Update()
        {
            float result = TriggerController.Update();

            result = _postProcessor.Process(result);

            if (_dynamicPostProcessor != null)
            {
                result = _dynamicPostProcessor.Process(result, Time.time);
            }
            
            Broadcast(_postProcessor.Process(result));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TriggerController?.Clear();
            TriggerController = null;
        }
    }
}