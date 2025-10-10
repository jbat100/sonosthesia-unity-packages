using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class VectorTouchDynamicExtractorSettings
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Velocity,
            Relative,
            Axis
        }
        
        public enum Space
        {
            World,
            Source,
            Actor
        }

        [Flags]
        public enum PostProcessingType
        {
            Normalize = 1 << 0,
            Scale = 1 << 2
        }
        
        [SerializeField] private ExtractorType _extractorType;

        [SerializeField] private DynamicExtractor<TouchEvent, Vector3> _extractor;

        [SerializeField] private TouchVelocityType _velocityType;
        
        [SerializeField] private Space _space;

        [SerializeField] private Vector3 _direction;

        [SerializeField] private PostProcessingType _postProcessing;

        [SerializeField] private float _scale = 1f;

        public IDynamicExtractorSession<TouchEvent, Vector3> MakeSession()
        {
            IDynamicExtractorSession<TouchEvent, Vector3> session = _extractorType switch
            {
                ExtractorType.Custom => _extractor.MakeSession(),
                ExtractorType.Constant => new ConstantSession(_space, _direction),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Relative => new RelativeSession(),
                ExtractorType.Axis => new AxisSession(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (_postProcessing.HasFlag(PostProcessingType.Normalize))
            {
                session = new NormalizeSession(session);
            }

            if (_postProcessing.HasFlag(PostProcessingType.Scale))
            {
                session = new ScaleSession(session, _scale);
            }

            return session;
        }

        public IDynamicExtractorSession<TouchEvent, Vector3> SetupSession(TouchEvent e, out Vector3 result)
        {
            IDynamicExtractorSession<TouchEvent, Vector3> session = MakeSession();
            session.Setup(e, out result);
            return session;
        }

        private class NormalizeSession : ExtractorSessionProcessor<TouchEvent, Vector3>
        {
            public NormalizeSession(IDynamicExtractorSession<TouchEvent, Vector3> session) : base(session)
            {
            }

            protected override Vector3 Process(TouchEvent touchEvent, Vector3 value) => value.normalized;
        }

        private class ScaleSession : ExtractorSessionProcessor<TouchEvent, Vector3>
        {
            private readonly float _scale;
            
            public ScaleSession(IDynamicExtractorSession<TouchEvent, Vector3> session, float scale) : base(session)
            {
                _scale = scale;
            }

            protected override Vector3 Process(TouchEvent touchEvent, Vector3 value) => value * _scale;
        }

        private class ConstantSession : StatelessExtractorSession<TouchEvent, Vector3>
        {
            private readonly Space _space;
            private readonly Vector3 _direction;
            
            public ConstantSession(Space space, Vector3 direction)
            {
                _space = space;
                _direction = direction;
            }
            
            protected override bool Extract(TouchEvent touchEvent, out Vector3 value)
            {
                value = _space switch
                {
                    Space.Source => touchEvent.touchData.Source.transform.TransformDirection(_direction),
                    Space.Actor => touchEvent.touchData.Actor.transform.TransformDirection(_direction),
                    _ => _direction
                };
                return true;
            }
        }

        private class VelocitySession : StatelessExtractorSession<TouchEvent, Vector3>
        {
            private readonly TouchVelocityType _velocityType;
            
            public VelocitySession(TouchVelocityType velocityType)
            {
                _velocityType = velocityType;
            }
            
            protected override bool Extract(TouchEvent touchEvent, out Vector3 value)
            {
                return TouchExtractionUtils.ExtractVelocity(touchEvent, _velocityType, out value);
            }
        }

        private class RelativeSession : StatelessExtractorSession<TouchEvent, Vector3>
        {
            protected override bool Extract(TouchEvent touchEvent, out Vector3 value)
            {
                value = touchEvent.touchData.Actor.transform.position - touchEvent.touchData.Source.transform.position;
                return true;
            }
        }

        private class AxisSession : StatelessExtractorSession<TouchEvent, Vector3>
        {
            protected override bool Extract(TouchEvent touchEvent, out Vector3 value)
            {
                Vector3 actorToSource = touchEvent.touchData.Source.transform.position - touchEvent.touchData.Actor.transform.position;
                Vector3 actorVelocity = touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position;
                value = Vector3.Cross(actorVelocity, actorToSource);
                return true;
            }
        }
    }
}