using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
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
        : DynamicExtractorSettings<TEvent, float, FloatFollowStrategy, FloatPostProcessingSettings>
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

    public abstract class InteractionFloatDynamicExtractorSettings<TEvent> : FloatDynamicExtractorSettings<TEvent> 
        where TEvent : IInteractionEvent
    {
        [SerializeField] private VelocityExtractionType _velocityType = VelocityExtractionType.Actor;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;

        protected IDynamicExtractorSession<TEvent, float> VelocitySession() => new VelocityFloatExtractorSession<TEvent>(_velocityType);
        protected IDynamicExtractorSession<TEvent, float> DistanceSession() => new ActorToSourceDistanceSession<TEvent>(_axes);
        protected IDynamicExtractorSession<TEvent, float> TwistSession() => new TwistFloatExtractorSession<TEvent>();
        protected IDynamicExtractorSession<TEvent, float> HeightSession() => new HeightFloatExtractorSession<TEvent>();
    }
}