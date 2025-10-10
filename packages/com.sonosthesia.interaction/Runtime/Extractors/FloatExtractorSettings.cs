using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public abstract class FloatExtractorSettings<TEvent> where TEvent : IInteractionEvent
    {
        [Flags]
        public enum PostProcessingType
        {
            Curve = 1 << 0,
            Remap = 1 << 1,
            Clamp = 1 << 2
        }
        
        private enum FollowStrategy
        {
            Initial,
            Track,
            Relative
        }

        public abstract IExtractorSession<TEvent, float> MakeSession();
        
        // used when only the initial value is needed, creates a session, sets it up and returns extracted value
        public bool Extract(TEvent e, out float value)
        {
            IExtractorSession<TEvent, float> session = MakeSession();
            return session.Setup(e, out value);
        }
        
        // ----------- custom -------------
        
        [SerializeField] private DynamicExtractor<TEvent, float> _extractor;
        protected IExtractorSession<TEvent, float> CustomSession() => _extractor.MakeSession();

        // ----------- static -------------
        
        [SerializeField] private float _staticValue = 1;
        protected IExtractorSession<TEvent, float> StaticSession() => new FloatStaticSession<TEvent>(_staticValue);
        
        // ----------- follow -------------

        [SerializeField] private FollowStrategy _followStrategy;
        
        // ----------- postprocess -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;

        protected IExtractorSession<TEvent, float> FollowSession(IExtractorSession<TEvent, float> session)
        {
            return _followStrategy switch
            {
                FollowStrategy.Initial => new InitialSession<TEvent, float>(session),
                FollowStrategy.Relative => new FloatRelativeSession<TEvent>(session),
                _ => session
            };
        }

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