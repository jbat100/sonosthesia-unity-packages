using System;
using Sonosthesia.Dynamic;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public interface IDynamicTrackingSession
    {
        Vector3 Update(float deltaTime);
    }
    
    public static class DynamicTrackingSessionUtil
    {
        public static IDynamicTrackingSession CreateSession(DynamicTrackingSettings settings, TransformDynamicsMonitor monitor)
        {
            return settings.Strategy switch 
            {
                DynamicTrackingStrategy.FreezePosition => new FreezePositionSession(monitor),
                DynamicTrackingStrategy.Follow => new FollowSession(monitor),
                DynamicTrackingStrategy.FreezeVelocity => new FreezeVelocitySession(monitor, settings.Drag),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private class FreezePositionSession : IDynamicTrackingSession
        {
            private readonly Vector3 _position;
            
            public FreezePositionSession(Component monitor)
            {
                _position = monitor.transform.position;
            }

            public Vector3 Update(float deltaTime) => _position;
        }

        private class FollowSession : IDynamicTrackingSession
        {
            private readonly Transform _transform;

            public FollowSession(Component monitor)
            {
                _transform = monitor.transform;
            }

            public Vector3 Update(float deltaTime) => _transform.position;
        }

        private class FreezeVelocitySession : IDynamicTrackingSession
        {
            private readonly float _drag;
            
            private Vector3 _currentVelocity;
            private Vector3 _currentPosition;

            public FreezeVelocitySession(TransformDynamicsMonitor monitor, float drag)
            {
                _drag = drag;
                _currentPosition = monitor.transform.position;
                _currentVelocity = monitor.Select(TransformDynamics.Order.Velocity).Position;
            }

            public Vector3 Update(float deltaTime)
            {
                _currentVelocity = _currentVelocity.ChangeLength(1f - _drag * deltaTime);
                _currentPosition += _currentVelocity * deltaTime;
                Debug.Log($"{this} {nameof(Update)} {nameof(_currentPosition)} {_currentPosition} {nameof(_currentVelocity)} {_currentVelocity}");
                return _currentPosition;
            }
        }
    }
}