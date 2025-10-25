using System;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum VectorFollowStrategy
    {
        Initial,
        Track,
        Relative
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
    
    [Serializable]
    public abstract class InteractionVectorDynamicExtractorSettings<TEvent> : VectorDynamicExtractorSettings<TEvent>
        where TEvent : IInteractionEvent
    {
        [SerializeField] private VelocityExtractionType _velocityType;
        
        [SerializeField] private ExtractionSpace _space;

        [SerializeField] private Vector3 _direction;
        
        protected ExtractionSpace Space => _space;
        
        protected IDynamicExtractorSession<TEvent, Vector3> DirectionSession() => new DirectionVectorExtractorSession<TEvent>(_space, _direction);
        protected IDynamicExtractorSession<TEvent, Vector3> VelocitySession() => new VelocityVectorExtractorSession<TEvent>(_velocityType);
        protected IDynamicExtractorSession<TEvent, Vector3> RelativeSession() => new RelativePositionVectorExtractionSession<TEvent>();
        protected IDynamicExtractorSession<TEvent, Vector3> AxisSession() => new AxisVectorExtractionSession<TEvent>();
    }
}