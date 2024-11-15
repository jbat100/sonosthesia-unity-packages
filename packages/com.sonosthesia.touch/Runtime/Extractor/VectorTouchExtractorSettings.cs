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
        
        [SerializeField] private ExtractorType _extractorType;

        [SerializeField] private TouchExtractor<Vector3> _extractor;

        [SerializeField] private VelocityType _velocityType;
        
        [SerializeField] private Space _space;

        [SerializeField] private Vector3 _direction;

        public ITouchExtractorSession<Vector3> MakeSession()
        {
            return _extractorType switch
            {
                ExtractorType.Custom => _extractor.MakeSession(),
                ExtractorType.Static => new StaticSession(_space, _direction),
                ExtractorType.Velocity => new VelocitySession(_velocityType),
                ExtractorType.Relative => new RelativeSession(),
                ExtractorType.Axis => new AxisSession(),
                _ => throw new ArgumentOutOfRangeException()
            };
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
                    Space.Source => touchEvent.TouchData.Source.transform.TransformDirection(_direction),
                    Space.Actor => touchEvent.TouchData.Actor.transform.TransformDirection(_direction),
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
                    VelocityType.Actor => touchEvent.TouchData.Actor.DynamicsMonitor.Velocity.Position,
                    VelocityType.Source => touchEvent.TouchData.Source.DynamicsMonitor.Velocity.Position,
                    VelocityType.Relative => touchEvent.TouchData.Actor.DynamicsMonitor.Velocity.Position -
                                         touchEvent.TouchData.Source.DynamicsMonitor.Velocity.Position,
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
                value = touchEvent.TouchData.Actor.transform.position - touchEvent.TouchData.Source.transform.position;
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }

        private class AxisSession : ITouchExtractorSession<Vector3>
        {
            private static bool Common(TouchEvent touchEvent, out Vector3 value)
            {
                Vector3 actorToSource = touchEvent.TouchData.Source.transform.position - touchEvent.TouchData.Actor.transform.position;
                Vector3 actorVelocity = touchEvent.TouchData.Actor.DynamicsMonitor.Velocity.Position;
                value = Vector3.Cross(actorVelocity, actorToSource);
                return true;
            }
            
            public bool Setup(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);

            public bool Update(TouchEvent touchEvent, out Vector3 value) => Common(touchEvent, out value);
        }
    }
}