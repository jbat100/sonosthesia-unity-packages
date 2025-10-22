using System;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public abstract class FloatDynamicExtractorSettings<TEvent> : FloatPostProcessingSettings, IDynamicExtractor<TEvent, float>
    {
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
        
        // ----------- custom -------------
        
        [SerializeField] private InterfaceReference<IDynamicExtractor<TEvent, float>> _extractor;
        protected IDynamicExtractorSession<TEvent, float> CustomSession() => _extractor.Value.MakeSession();

        // ----------- static -------------
        
        [SerializeField] private float _constantValue = 1;
        protected IDynamicExtractorSession<TEvent, float> ConstantSession() => new FloatConstantSession<TEvent>(_constantValue);
        
        // ----------- follow -------------

        [SerializeField] private FollowStrategy _followStrategy;
        
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
            return new FloatFuncSession<TEvent>(session, PostProcess);;
        }
    }
}