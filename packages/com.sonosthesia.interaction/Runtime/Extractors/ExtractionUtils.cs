using System;
using Sonosthesia.Extractor;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public enum InteractionExtractorOrigin
    {
        Self,
        Source,
        Actor
    }
    
    public enum ExtractionSpace
    {
        World,
        Source,
        Actor
    }
    
    public enum VelocityExtractionType
    {
        Actor,
        Source,
        Relative
    }
    
    public static class ExtractionUtils
    {
        public static TComponent GetComponent<TEvent, TComponent>(this TEvent e, InteractionExtractorOrigin origin, TComponent self)
            where TEvent : IInteractionEvent 
        {
            return origin switch
            {
                InteractionExtractorOrigin.Self => self,
                InteractionExtractorOrigin.Source => e.Source.Transform.GetComponent<TComponent>(),
                InteractionExtractorOrigin.Actor => e.Actor.Transform.GetComponent<TComponent>(),
                _ => throw new NotSupportedException()
            };
        }
        
        public static bool WorldToExtractionSpace<TEvent>(this TEvent e, ExtractionSpace extractionSpace, Vector3 point, out Vector3 result)
            where TEvent : IInteractionEvent
        {
            result = default;
            switch (extractionSpace)
            {
                case ExtractionSpace.World:
                    result = point;
                    return true;
                case ExtractionSpace.Actor when e.Actor?.Transform !=null:
                    result = e.Actor.Transform.InverseTransformPoint(point);
                    return true;
                case ExtractionSpace.Source when e.Source?.Transform != null:
                    result = e.Source.Transform.InverseTransformPoint(point);
                    return true;
                default:
                    return false;
            }
        }
        
        // consider space argument
        public static bool ExtractRelativePosition<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            if (!e.HasValidTransforms())
            {
                value = default;
                return false;
            }
            value = e.Actor.Transform.position - e.Source.Transform.position;
            return true;
        }

        public static bool ExtractDistance<TEvent>(this TEvent e, Axes axes, out float value)
            where TEvent : IInteractionEvent
        {
            return e.ActorToSourceDistance(axes, out value);
        }
        
        public static bool ExtractAxis<TEvent>(this TEvent e, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            if (!e.HasValidTransforms() || !e.Actor.DynamicsMonitor)
            {
                value = default;
                return false;
            }
            Vector3 actorToSource = e.Source.Transform.position - e.Actor.Transform.position;
            Vector3 actorVelocity = e.Actor.DynamicsMonitor.Velocity.Position;
            value = Vector3.Cross(actorVelocity, actorToSource);
            return true;
        }
        
        public static bool TransformPoint<TEvent>(this TEvent e, ExtractionSpace space, Vector3 direction, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = default;
            switch (space)
            {
                case ExtractionSpace.Actor when e.Actor?.Transform != null:
                    value = e.Actor.Transform.TransformPoint(direction);
                    return true;
                case ExtractionSpace.Source when e.Source?.Transform != null:
                    value = e.Source.Transform.TransformPoint(direction);
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool ExtractVelocity<TEvent>(this TEvent e, VelocityExtractionType velocityType, out float value)
            where TEvent : IInteractionEvent
        {
            if (ExtractVelocity(e, velocityType, out Vector3 velocity))
            {
                value = velocity.magnitude;
                return true;
            }
            value = 0;
            return false;
        }
        
        public static bool ExtractVelocity<TEvent>(this TEvent e, VelocityExtractionType velocityType, out Vector3 value)
            where TEvent : IInteractionEvent
        {
            value = default;
            switch (velocityType)
            {
                case VelocityExtractionType.Actor when e.Actor?.DynamicsMonitor != null:
                    value = e.Actor.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Source when e.Source?.DynamicsMonitor != null:
                    value = e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                case VelocityExtractionType.Relative when e.Actor?.DynamicsMonitor != null && e.Source?.DynamicsMonitor != null:
                    value = e.Actor.DynamicsMonitor.Velocity.Position - e.Source.DynamicsMonitor.Velocity.Position;
                    return true;
                default:
                    return false;
            }
        }
    }
    
    public class VelocityFloatExtractorSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly VelocityExtractionType _type;
            
        public VelocityFloatExtractorSession(VelocityExtractionType type)
        {
            _type = type;
        }

        protected override bool Extract(TEvent e, out float value) => e.ExtractVelocity(_type, out value);
    }
    
    public class DirectionVectorExtractorSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        private readonly ExtractionSpace _space;
        private readonly Vector3 _direction;
            
        public DirectionVectorExtractorSession(ExtractionSpace space, Vector3 direction)
        {
            _space = space;
            _direction = direction;
        }

        protected override bool Extract(TEvent e, out Vector3 value) => e.TransformPoint(_space, _direction, out value);
    }

    public class VelocityVectorExtractorSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        private readonly VelocityExtractionType _velocityType;
            
        public VelocityVectorExtractorSession(VelocityExtractionType velocityType)
        {
            _velocityType = velocityType;
        }
            
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractVelocity(_velocityType, out value);
    }
    


    public class RelativePositionVectorExtractionSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractRelativePosition(out value);
    }

    public class AxisVectorExtractionSession<TEvent> : StatelessExtractorSession<TEvent, Vector3> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out Vector3 value) => e.ExtractAxis(out value);
    }
    
    public class ActorToSourceDistanceSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private readonly Axes _axes;
            
        public ActorToSourceDistanceSession(Axes axes)
        {
            _axes = axes;
        }

        protected override bool Extract(TEvent e, out float value) => e.ActorToSourceDistance(_axes, out value);
    }

    public class HeightFloatExtractorSession<TEvent> : StatelessExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        protected override bool Extract(TEvent e, out float value)
        {
            value = e.Actor.Transform.position.y;
            return true;
        }
    }

    public class TwistFloatExtractorSession<TEvent> : IDynamicExtractorSession<TEvent, float> where TEvent : IInteractionEvent
    {
        private Quaternion _referenceRotation;

        private bool Common(TEvent e, out float value)
        {
            Quaternion rotation = e.Actor.Transform.rotation;
            value = Quaternion.Angle(_referenceRotation, rotation) / 180f;
            return true;
        }
            
        public bool Setup(TEvent e, out float value)
        {
            _referenceRotation = e.Actor.Transform.rotation;
            return Common(e, out value);
        }

        public bool Update(TEvent e, out float value) => Common(e, out value);
    }
    
    
    [Serializable]
    public class FloatInteractionDynamicExtractorSettings<TEvent> : FloatDynamicExtractorSettings<TEvent> 
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
    
    
    public abstract class InteractionStaticExtractorSettings<TEvent, TValue, TProcessing>
        : StaticExtractorSettings<TEvent, TValue, TProcessing>
        where TEvent : IInteractionEvent
        where TValue : struct
        where TProcessing : IPostProcessing<TValue>
    {
        [SerializeField] private VelocityExtractionType _velocityType = VelocityExtractionType.Actor;
        
        [SerializeField] private Axes _axes = Axes.X | Axes.Y | Axes.Z;
        
        protected bool ExtractVelocity(TEvent e, out float value) => e.ExtractVelocity(_velocityType, out value);
        protected bool ExtractDistance(TEvent e, out float value) => e.ExtractDistance(_axes, out value);
    }

    // can't nest in FloatInteractionStaticExtractorSettings because need to refer to it in template editor
    public enum FloatInteractionStaticExtractorType
    {
        Custom,
        Constant,
        Velocity,
        Distance
    }
    
    public class FloatInteractionStaticExtractorSettings<TEvent, TProcessing>
        : InteractionStaticExtractorSettings<TEvent, float, TProcessing>
        where TEvent : IInteractionEvent
        where TProcessing : IPostProcessing<float>
    {
        [SerializeField] private FloatInteractionStaticExtractorType _extractorType 
            = FloatInteractionStaticExtractorType.Constant;
        
        protected override bool ExtractRaw(TEvent e, out float value) => _extractorType switch
        {
            FloatInteractionStaticExtractorType.Custom => ExtractCustom(e, out value),
            FloatInteractionStaticExtractorType.Constant => ExtractConstant(e, out value),
            FloatInteractionStaticExtractorType.Velocity => ExtractVelocity(e, out value),
            FloatInteractionStaticExtractorType.Distance => ExtractDistance(e, out value),
            _ => throw new ArgumentOutOfRangeException()
        };
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
    
    public abstract class InteractionVectorStaticExtractorSettings<TEvent> : VectorStaticExtractorSettings<TEvent>
        where TEvent : IInteractionEvent
    {
        [SerializeField] private VelocityExtractionType _velocityType;
        
        [SerializeField] private ExtractionSpace _space;

        [SerializeField] private Vector3 _direction;
        
        protected bool ExtractDirection(TEvent e, out Vector3 value) => e.TransformPoint(_space, _direction, out value);
        protected bool ExtractVelocity(TEvent e, out Vector3 value) => e.ExtractVelocity(_velocityType, out value);
        protected bool ExtractRelative(TEvent e, out Vector3 value) => e.ExtractRelativePosition(out value);
        protected bool ExtractAxis(TEvent e, out Vector3 value) => e.ExtractAxis(out value);
    }
}