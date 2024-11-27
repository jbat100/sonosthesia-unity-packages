using Sonosthesia.Envelope;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public class SignalTriggerConfiguration<T, TExtractor> : ScriptableObject where T : struct where TExtractor : IExtractor<T>
    {
        [SerializeField] private TExtractor _value;
        
        [SerializeField] private TExtractor _time;

        [SerializeField] private EnvelopeSettings _envelope;

        public void Trigger(TriggerController controller, T input)
        {
            float value = _value.Extract(input);
            float time = _time.Extract(input);
            IEnvelope envelope = _envelope.Build();
            
            controller.PlayTrigger(envelope, value, time);
        }
    }
}