using Sonosthesia.Envelope;
using Sonosthesia.Extractor;
using UnityEngine;

namespace Sonosthesia.Trigger
{
    public interface ISignalTriggerConfiguration<in T> where T : struct
    {
        IStaticExtractor<T, float> ValueExtractor { get; }
        IStaticExtractor<T, float> AttackExtractor { get; }
        EnvelopeSettings Envelope { get; }
    }

    public static class SignalTriggerConfigurationExtensions
    {
        public static void Trigger<T>(this ISignalTriggerConfiguration<T> configuration, TriggerImplementation controller, T input) where T : struct
        {
            if (!configuration.ValueExtractor.Extract(input, out float value) || !configuration.AttackExtractor.Extract(input, out float time))
            {
                return;
            }
            IEnvelope envelope = configuration.Envelope.Build();
            controller.StartTrigger(envelope, value, time, true);
        }
    }
    
    public class SignalTriggerConfiguration<T, TExtractor> : ScriptableObject, ISignalTriggerConfiguration<T> 
        where T : struct where TExtractor : IStaticExtractor<T, float>
    {
        [SerializeField] private TExtractor _value;
        public IStaticExtractor<T, float> ValueExtractor => _value;
        
        [SerializeField] private TExtractor _attack;
        public IStaticExtractor<T, float> AttackExtractor => _attack;

        [SerializeField] private EnvelopeSettings _envelope;
        public EnvelopeSettings Envelope => _envelope;
    }
}