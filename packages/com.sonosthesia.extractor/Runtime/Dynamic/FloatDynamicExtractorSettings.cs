using System;
using Sonosthesia.Processing;

namespace Sonosthesia.Extractor
{
    public enum FloatFollowStrategy
    {
        Initial,
        Track,
        Relative,
        Normalized
    }    
    
    [Serializable]
    public abstract class FloatDynamicExtractorSettings<TEvent> 
        : DynamicExtractorSettings<TEvent, float, FloatFollowStrategy, FloatProcessorSettings>
    {
        protected override IDynamicExtractorSession<TEvent, float> FollowSession(FloatFollowStrategy follow,
            IDynamicExtractorSession<TEvent, float> session)
        {
            return follow switch
            {
                FloatFollowStrategy.Initial => new InitialSession<TEvent, float>(session),
                FloatFollowStrategy.Relative => new FloatRelativeSession<TEvent>(session),
                FloatFollowStrategy.Normalized => new FloatNormalizedSession<TEvent>(session),
                _ => session
            };
        }
    }

}