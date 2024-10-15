using Sonosthesia.Envelope;
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

        private TriggerController _triggerController;

        private IEnvelope _defaultEnvelope;
        protected virtual IEnvelope DefaultEnvelope =>
            _defaultEnvelope ??= new AHREnvelope(EnvelopePhase.Linear(0.25f), 0.5f, EnvelopePhase.Linear(0.25f));
        
        public void StartTrigger(float valueScale, float timeScale) => StartTrigger(DefaultEnvelope, valueScale, timeScale);

        public void StartTrigger(IEnvelope envelope, float valueScale, float timeScale)
        {
            _triggerController.StartTrigger(envelope, valueScale, timeScale);
        }

        public int TriggerCount => _triggerController.TriggerCount; 

        private void SetupState()
        {
            _triggerController = new TriggerController(_accumulationMode);
            _dynamicPostProcessor = _postProcessorFactory ? _postProcessorFactory.Make() : null;
        }

        protected virtual void OnValidate() => SetupState();

        protected virtual void OnEnable() => SetupState();
        
        protected virtual void Update()
        {
            float result = _triggerController.Update();

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
            _triggerController?.Clear();
            _triggerController = null;
        }
        
    }
}