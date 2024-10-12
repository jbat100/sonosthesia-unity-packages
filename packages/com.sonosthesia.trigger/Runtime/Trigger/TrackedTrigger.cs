using System;
using Sonosthesia.Envelope;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TrackedTrigger), true)]
    public class TrackedTriggerableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TrackedTrigger trigger = (TrackedTrigger)target;
            if(GUILayout.Button("End All"))
            {
                trigger.EndAll();
            }
        }
    }
#endif
    
    public class TrackedTrigger : Signal<float>
    {
        [SerializeField] private AccumulationMode _accumulationMode = AccumulationMode.Max;

        [SerializeField] private FloatProcessor _postProcessor;

        private TrackedTriggerController _triggerController;

        private IEnvelope _defaultStartEnvelope;
        protected virtual IEnvelope DefaultStartEnvelope => 
            _defaultStartEnvelope ??= new ADSEnvelope(EnvelopePhase.Linear(0.5f), EnvelopePhase.Linear(0.5f), 0.5f);

        private IEnvelope _defaultEndEnvelope;
        protected virtual IEnvelope DefaultEndEnvelope => 
            _defaultEndEnvelope ??= new SREnvelope(1f, EnvelopePhase.Linear(0.5f));

        public Guid StartTrigger(IEnvelope envelope, float valueScale, float timeScale)
        {
            Guid id = Guid.NewGuid();
            StartTrigger(id, envelope, valueScale, timeScale);
            return id;
        }
        
        public void StartTrigger(Guid id, IEnvelope envelope, float valueScale, float timeScale)
        {
            _triggerController.StartTrigger(id, envelope ?? DefaultStartEnvelope, valueScale, timeScale);
        }
        
        public Guid StartTrigger(float valueScale, float timeScale) 
            => StartTrigger(DefaultStartEnvelope, valueScale, timeScale);

        public void StartTrigger(Guid id, float valueScale, float timeScale)
            => StartTrigger(id, DefaultEndEnvelope, valueScale, timeScale);

        public bool UpdateTrigger(Guid id, float valueScale)
            => _triggerController.UpdateTrigger(id, valueScale);


        public bool EndTrigger(Guid id, IEnvelope envelope, float timescale = 1f)
            => _triggerController.EndTrigger(id, envelope ?? DefaultEndEnvelope, timescale);
        
        public void EndTrigger(Guid id, float timescale = 1f) => EndTrigger(id, DefaultEndEnvelope, timescale);

        public void EndAll(IEnvelope envelope, float timescale = 1) =>
            _triggerController.EndAll(envelope ?? DefaultEndEnvelope, timescale);

        public void EndAll(float timeScale = 1f) => EndAll(DefaultEndEnvelope, timeScale);
        
        protected virtual void Update()
        {
            float raw = _triggerController.Update();
            Broadcast(_postProcessor.Process(raw));
        }

        protected virtual void OnEnable() => SetupState();
        
        protected virtual void OnValidate() => SetupState();
        
        protected virtual void OnDisable()
        {
            _triggerController?.Dispose();
            Broadcast(0);
        }
        
        private void SetupState()
        {
            _triggerController?.Dispose();
            _triggerController = new TrackedTriggerController(_accumulationMode);
        }
    }
}