using System;
using Sonosthesia.Envelope;

namespace Sonosthesia.Trigger
{
    public class TriggerController
    {
        private readonly PlayTriggerImplementation _playTriggerController;
        private readonly TrackedTriggerImplementation _trackedTriggerController;
        
        public TriggerController(AccumulationMode accumulationMode)
        {
            _playTriggerController = new PlayTriggerImplementation(accumulationMode);
            _trackedTriggerController = new TrackedTriggerImplementation(accumulationMode);
        }
        
        public float Update()
        {
            return _playTriggerController.Update() + _trackedTriggerController.Update();
        }

        public void Clear()
        {
            _playTriggerController.Clear();
            _trackedTriggerController.Clear();
        }

        public void PlayTrigger(IEnvelope envelope, float valueScale, float timeScale)
        {
            _playTriggerController.PlayTrigger(envelope, valueScale, timeScale);
        }
        
        public Guid StartTrigger(IEnvelope envelope, float valueScale, float timeScale)
        {
            return _trackedTriggerController.StartTrigger(envelope, valueScale, timeScale);
        }
        
        public void StartTrigger(Guid id, IEnvelope envelope, float valueScale, float timeScale)
        {
            _trackedTriggerController.StartTrigger(id, envelope, valueScale, timeScale);
        }
        
        public bool UpdateTrigger(Guid id, float valueScale)
        {
            return _trackedTriggerController.UpdateTrigger(id, valueScale);
        }

        public bool EndTrigger(Guid id, IEnvelope envelope, float timescale = 1f)
        {
            return _trackedTriggerController.EndTrigger(id, envelope, timescale);
        }
        
        public void EndAll(IEnvelope envelope, float timescale = 1)
        {
            _trackedTriggerController.EndAll(envelope, timescale);
        }
        
    }
}