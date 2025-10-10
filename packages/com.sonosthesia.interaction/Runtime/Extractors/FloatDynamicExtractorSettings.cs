using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public abstract class FloatDynamicExtractorSettings<TEvent> : IDynamicExtractor<TEvent, float> where TEvent : IInteractionEvent
    {
        [Flags]
        public enum PostProcessingType
        {
            Curve = 1 << 0,
            Remap = 1 << 1,
            Clamp = 1 << 2
        }
        
        public enum FollowStrategy
        {
            Initial,
            Track,
            Relative,
            Normalized
        }

        public IDynamicExtractorSession<TEvent, float> MakeSession()
        {
            IDynamicExtractorSession<TEvent, float> session = MakeRawSession();

            if (!BypassFollow)
            {
                session = FollowSession(session);
            }
            
            if (!BypassPostProcess)
            {
                session = PostProcessSession(session);   
            }
            
            return session;
        }

        protected virtual bool BypassFollow => false;
        
        protected virtual bool BypassPostProcess => false;
        
        protected abstract IDynamicExtractorSession<TEvent, float> MakeRawSession();
        
        // used when only the initial value is needed, creates a session, sets it up and returns extracted value
        public bool Extract(TEvent e, out float value)
        {
            IDynamicExtractorSession<TEvent, float> session = MakeSession();
            return session.Setup(e, out value);
        }
        
        // ----------- custom -------------
        
        [SerializeField] private DynamicExtractor<TEvent, float> _extractor;
        protected IDynamicExtractorSession<TEvent, float> CustomSession() => _extractor.MakeSession();

        // ----------- static -------------
        
        [SerializeField] private float _constantValue = 1;
        protected IDynamicExtractorSession<TEvent, float> ConstantSession() => new FloatConstantSession<TEvent>(_constantValue);
        
        // ----------- follow -------------

        [SerializeField] private FollowStrategy _followStrategy;
        
        // ----------- postprocess -------------
        
        [SerializeField] private PostProcessingType _postProcessing;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RemapSettings _remap;
        [SerializeField] private FloatRange _clamp;

        private IDynamicExtractorSession<TEvent, float> FollowSession(IDynamicExtractorSession<TEvent, float> session)
        {
            return _followStrategy switch
            {
                FollowStrategy.Initial => new InitialSession<TEvent, float>(session),
                FollowStrategy.Relative => new FloatRelativeSession<TEvent>(session),
                FollowStrategy.Normalized => new FloatNormalizedSession<TEvent>(session),
                _ => session
            };
        }

        private IDynamicExtractorSession<TEvent, float> PostProcessSession(IDynamicExtractorSession<TEvent, float> session)
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