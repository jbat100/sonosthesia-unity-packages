using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public class FloatExtractorSettings<TEvent> where TEvent : IInteractionEvent
    {
        [Flags]
        public enum PostProcessingType
        {
            Curve = 1 << 0,
            Remap = 1 << 1,
            Clamp = 1 << 2
        }
        
        // ----------- custom -------------
        
        [SerializeField] private Extractor<TEvent, float> _extractor;
        protected IExtractorSession<TEvent, float> CustomSession() => _extractor.MakeSession();

        // ----------- static -------------
        
        [SerializeField] private float _staticValue = 1;
        protected IExtractorSession<TEvent, float> StaticSession() => new FloatStaticSession<TEvent>(_staticValue);
        
        // ----------- postprocess -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;

        protected IExtractorSession<TEvent, float> PostProcessSession(IExtractorSession<TEvent, float> session)
        {
            if (_postProcessing.HasFlag(PostProcessingType.Curve))
            {
                session = new FloatCurveSession<TEvent>(session, _curve);
            }

            if (_postProcessing.HasFlag(PostProcessingType.Remap))
            {
                session = new FloatRemapSession<TEvent>(session, _remap);
            }

            if (_postProcessing.HasFlag(PostProcessingType.Clamp))
            {
                session = new FloatClampSession<TEvent>(session, _clamp);
            }

            return session;
        }
    }
}