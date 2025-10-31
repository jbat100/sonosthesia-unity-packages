using System;
using UnityEngine;

namespace Sonosthesia.Extractor
{
    [Serializable]
    public abstract class DynamicExtractorSettings<TEvent, TValue, TFollow, TProcessing> : IDynamicExtractor<TEvent, TValue>
        where TValue : struct
        where TProcessing : IPostProcessing<TValue>
    {
        [SerializeField] private TProcessing _postProcessing;
        
        [SerializeField] private InterfaceReference<IDynamicExtractor<TEvent, TValue>> _extractor;
        
        [SerializeField] private TFollow _followStrategy;
        
        [SerializeField] private TValue _constantValue;
        
        public IDynamicExtractorSession<TEvent, TValue> MakeSession()
        {
            IDynamicExtractorSession<TEvent, TValue> session = MakeRawSession();

            if (!BypassFollow)
            {
                session = FollowSession(_followStrategy, session);
            }

            if (!BypassModulate)
            {
                session = ModulateSession(session);
            }
            
            if (!BypassPostProcess)
            {
                session = new FuncExtractionSessionProcessor<TEvent, TValue>(session, _postProcessing.PostProcess);
            }
            
            return session;
        }
        

        
        protected virtual bool BypassFollow => false;
        protected virtual bool BypassPostProcess => false;
        protected virtual bool BypassModulate => false;
        
        protected abstract IDynamicExtractorSession<TEvent, TValue> MakeRawSession();
        protected abstract IDynamicExtractorSession<TEvent, TValue> FollowSession(
            TFollow follow, IDynamicExtractorSession<TEvent, TValue> session);

        protected virtual IDynamicExtractorSession<TEvent, TValue> ModulateSession(
            IDynamicExtractorSession<TEvent, TValue> session)
            => session;
        
        protected IDynamicExtractorSession<TEvent, TValue> CustomSession() 
            => _extractor.Value.MakeSession();
        protected IDynamicExtractorSession<TEvent, TValue> ConstantSession() 
            => new ConstantExtractorSession<TEvent, TValue>(_constantValue);
    }
}