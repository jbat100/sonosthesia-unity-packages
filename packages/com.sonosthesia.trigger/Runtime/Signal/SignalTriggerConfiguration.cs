using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public interface ISignalTriggerConfiguration<in T> where T : struct
    {
        IStaticExtractor<T, float> ValueExtractor { get; }
        IStaticExtractor<T, float> TimeExtractor { get; }
        EnvelopeSettings Envelope { get; }
    }

    public static class SignalTriggerConfigurationExtensions
    {
        public static void Trigger<T>(this ISignalTriggerConfiguration<T> configuration, TriggerController controller, T input) where T : struct
        {
            if (!configuration.ValueExtractor.Extract(input, out float value) || !configuration.TimeExtractor.Extract(input, out float time))
            {
                return;
            }
            IEnvelope envelope = configuration.Envelope.Build();
            controller.PlayTrigger(envelope, value, time);
        }
    }
    
    public class SignalTriggerConfiguration<T, TExtractor> : ScriptableObject, ISignalTriggerConfiguration<T> 
        where T : struct where TExtractor : IStaticExtractor<T, float>
    {
        [SerializeField] private TExtractor _value;
        public IStaticExtractor<T, float> ValueExtractor => _value;
        
        [SerializeField] private TExtractor _time;
        public IStaticExtractor<T, float> TimeExtractor => _time;

        [SerializeField] private EnvelopeSettings _envelope;
        public EnvelopeSettings Envelope => _envelope;
    }
}