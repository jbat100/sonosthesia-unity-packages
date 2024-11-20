using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class VectorTouchExtractorSettings
    {
        public enum ExtractorType
        {
            Custom,
            Static,
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

        public enum VelocityType
        {
            Actor,
            Source,
            Relative
        }

        [Flags]
        public enum PostProcessingType
        {
            Normalize = 1 << 0,
            Scale = 1 << 2
        }
        
        [SerializeField] private ExtractorType _extractorType;

        [SerializeField] private TouchExtractor<Vector3> _extractor;

        [SerializeField] private VelocityType _velocityType;
        
        [SerializeField] private Space _space;

        [SerializeField] private Vector3 _direction;

        [SerializeField] private PostProcessingType _postProcessing;

        [SerializeField] private float _scale = 1f;

        public ITouchExtractorSession<Vector3> MakeSession()
        {
            ITouchExtractorSession<Vector3> session = _extractorType switch
            {
                ExtractorType.Custom => _extractor.MakeSession(),
                ExtractorType.Static => new StaticSession(_space, _direction),
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

        public ITouchExtractorSession<Vector3> SetupSession(TouchEvent e, out Vector3 result)
        {
            ITouchExtractorSession<Vector3> session = MakeSession();
            session.Setup(e, out result);
            return session;
        }

        private class NormalizeSession : TouchExtractorSessionProcessor<Vector3>
        {
            public NormalizeSession(ITouchExtractorSession<Vector3> session) : base(session)
            {
            }

            protected override Vector3 Process(TouchEvent touchEvent, Vector3 value) => value.normalized;
        }

        private class ScaleSession : TouchExtractorSessionProcessor<Vector3>
        {
            private readonly float _scale;
            
            public ScaleSession(ITouchExtractorSession<Vector3> session, float scale) : base(session)
            {
                _scale = scale;
            }

            protected override Vector3 Process(TouchEvent touchEvent, Vector3 value) => value * _scale;
        }

        private class StaticSession : ITouchExtractorSession<Vector3>
        {
            private readonly Space _space;
            private readonly Vector3 _direction;
            
            public StaticSession(Space space, Vector3 direction)
            {
                _space = space;
                _direction = direction;
            }
            
            private bool Common(TouchEvent touchEvent, out Vector3 value)
            {
                value = _space switch
                {
                    Space.Source => touchEvent.touchData.Source.transform.TransformDirection(_direction),
                    Space.Actor => touchEvent.touchData.Actor.transform.TransformDirection(_direction),
                    _ => _direction
                };
                return true;
            }

            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }

        private class VelocitySession : ITouchExtractorSession<Vector3>
        {
            private readonly VelocityType _velocityType;
            
            public VelocitySession(VelocityType velocityType)
            {
                _velocityType = velocityType;
            }
            
            private bool Common(TouchEvent touchEvent, out Vector3 value)
            {
                value = _velocityType switch
                {
                    VelocityType.Actor => touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position,
                    VelocityType.Source => touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position,
                    VelocityType.Relative => touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position -
                                         touchEvent.touchData.Source.DynamicsMonitor.Velocity.Position,
                    _ => Vector3.zero
                };

                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }

        private class RelativeSession : ITouchExtractorSession<Vector3>
        {
            private static bool Common(TouchEvent touchEvent, out Vector3 value)
            {
                value = touchEvent.touchData.Actor.transform.position - touchEvent.touchData.Source.transform.position;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }

        private class AxisSession : ITouchExtractorSession<Vector3>
        {
            private static bool Common(TouchEvent touchEvent, out Vector3 value)
            {
                Vector3 actorToSource = touchEvent.touchData.Source.transform.position - touchEvent.touchData.Actor.transform.position;
                Vector3 actorVelocity = touchEvent.touchData.Actor.DynamicsMonitor.Velocity.Position;
                value = Vector3.Cross(actorVelocity, actorToSource);
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }
    }
}