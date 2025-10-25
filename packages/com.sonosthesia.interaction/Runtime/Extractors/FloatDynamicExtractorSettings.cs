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

    [Serializable]
    public abstract class FloatInteractionDynamicExtractorSettings<TEvent> : FloatDynamicExtractorSettings<TEvent> 
        where TEvent : IInteractionEvent
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity,
            Distance,
            Twist,
            Height
        }
        
        [SerializeField] private ExtractorType _extractorType = ExtractorType.Constant;
        
        [SerializeField] private VelocityExtractionType _velocityType = VelocityExtractionType.Actor;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;
        
        protected override bool BypassFollow => _extractorType == ExtractorType.Constant;
        
        protected override IDynamicExtractorSession<TEvent, float> MakeRawSession() => _extractorType switch
        {
            ExtractorType.Custom => CustomSession(),
            ExtractorType.Constant => ConstantSession(),
            ExtractorType.Velocity => new VelocityFloatExtractorSession<TEvent>(_velocityType),
            ExtractorType.Distance => new ActorToSourceDistanceSession<TEvent>(_axes),
            ExtractorType.Twist => new TwistFloatExtractorSession<TEvent>(),
            ExtractorType.Height => new HeightFloatExtractorSession<TEvent>(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}