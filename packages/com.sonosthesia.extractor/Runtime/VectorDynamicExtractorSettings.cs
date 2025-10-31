using System;
using UnityEngine;

namespace Sonosthesia.Extractor
{
    public enum VectorFollowStrategy
    {
        Initial,
        Track,
        Relative
    }
    
    public class VectorRelativeSession<TEvent> : RelativeSession<TEvent, Vector3>
    {
        public VectorRelativeSession(IDynamicExtractorSession<TEvent, Vector3> session) : base(session)
        {
        }

        protected override Vector3 Relative(Vector3 value, Vector3 reference) => value - reference;
    }
    
    [Serializable]
    public abstract class VectorDynamicExtractorSettings<TEvent> 
        : DynamicExtractorSettings<TEvent, Vector3, VectorFollowStrategy, VectorPostProcessingSettings> 
    {
        protected override IDynamicExtractorSession<TEvent, Vector3> FollowSession(VectorFollowStrategy follow, 
            IDynamicExtractorSession<TEvent, Vector3> session)
        {
            return follow switch
            {
                VectorFollowStrategy.Initial => new InitialSession<TEvent, Vector3>(session),
                VectorFollowStrategy.Relative => new VectorRelativeSession<TEvent>(session),
                _ => session
            };
        }
    }
    
}