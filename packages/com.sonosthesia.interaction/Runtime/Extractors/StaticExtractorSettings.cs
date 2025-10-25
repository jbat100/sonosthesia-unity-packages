using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    // used for extracting a value based on a single TEvent, usually the first in the event stream 
    // for situations where after touch is not possible (e.g. MIDI note, velocity or channel selection) 
    
    public abstract class StaticExtractorSettings<TEvent, TValue, TProcessing> : IStaticExtractor<TEvent, TValue>
        where TValue : struct
        where TProcessing : IPostProcessing<TValue>
    {
        [SerializeField] private TProcessing _postProcessing;
        
        [SerializeField] private InterfaceReference<IStaticExtractor<TEvent, TValue>> _extractor;
        
        [SerializeField] private TValue _constantValue;
        
        protected bool ExtractCustom(TEvent e, out TValue value) => _extractor.Value.Extract(e, out value);

        protected bool ExtractConstant(TEvent e, out TValue value)
        {
            value = _constantValue;
            return true; 
        }

        public bool Extract(TEvent e, out TValue value)
        {
            if (!ExtractRaw(e, out value))
            {
                return false;
            }

            value = _postProcessing.PostProcess(value);
            return true;
        }

        protected abstract bool ExtractRaw(TEvent e, out TValue value);
    }

    public abstract class InteractionStaticExtractorSettings<TEvent, TValue, TProcessing>
        : StaticExtractorSettings<TEvent, TValue, TProcessing>
        where TEvent : IInteractionEvent
        where TValue : struct
        where TProcessing : IPostProcessing<TValue>
    {
        [SerializeField] private VelocityExtractionType _velocityType = VelocityExtractionType.Actor;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;
        
        protected bool ExtractVelocity(TEvent e, out float value) => e.ExtractVelocity(_velocityType, out value);
        protected bool ExtractDistance(TEvent e, out float value) => e.ExtractDistance(_axes, out value);
    }

    // can't nest in FloatInteractionStaticExtractorSettings because need to refer to it in template editor
    public enum FloatInteractionStaticExtractorType
    {
        Custom,
        Constant,
        Velocity,
        Distance
    }
    
    public class FloatInteractionStaticExtractorSettings<TEvent, TProcessing>
        : InteractionStaticExtractorSettings<TEvent, float, TProcessing>
        where TEvent : IInteractionEvent
        where TProcessing : IPostProcessing<float>
    {
        [SerializeField] private FloatInteractionStaticExtractorType _extractorType 
            = FloatInteractionStaticExtractorType.Constant;
        
        protected override bool ExtractRaw(TEvent e, out float value) => _extractorType switch
        {
            FloatInteractionStaticExtractorType.Custom => ExtractCustom(e, out value),
            FloatInteractionStaticExtractorType.Constant => ExtractConstant(e, out value),
            FloatInteractionStaticExtractorType.Velocity => ExtractVelocity(e, out value),
            FloatInteractionStaticExtractorType.Distance => ExtractDistance(e, out value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}