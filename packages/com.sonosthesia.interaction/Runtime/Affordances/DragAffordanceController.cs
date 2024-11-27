using System;
using Sonosthesia.Interaction;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sonosthesia.Pointer
{
    public abstract class DragAffordanceController<TEvent, TAffordance> : AffordanceController<TEvent, TAffordance> 
            where TEvent : struct, IInteractionEvent 
            where TAffordance : DragAgnosticAffordance<TEvent>
    {
        private GameObject _root;
        private Transform _origin;
        private Transform _target;
        private LineRenderer _lineRenderer;

        private Vector3 _originPosition;
        private Vector3 _targetPosition;
        
        private Camera _camera;
        private readonly Vector3[] _lineRendererPositions = new Vector3[2];
        
        public DragAffordanceController(Guid eventId, TAffordance affordance) : base(eventId, affordance)
        {
        }

        protected abstract bool GetOriginPosition(bool initial, TEvent value, ref Vector3 origin);
        protected abstract bool GetTargetPosition(bool initial, TEvent value, Vector3 origin, ref Vector3 target);

        protected abstract bool GetOriginScale(bool initial, TEvent value, ref Vector3 origin);
        
        protected abstract bool GetTargetScale(bool initial, TEvent value, Vector3 origin, ref Vector3 target);
        
        protected override void Setup(TEvent e)
        {
            base.Setup(e);
            _root = new GameObject(EventId.ToString());
            _root.transform.SetParent(Affordance.transform);
            if (Affordance.OriginPrefab)
            {
                _origin = Object.Instantiate(Affordance.OriginPrefab, _root.transform);
            }
            if (Affordance.TargetPrefab)
            {
                _target = Object.Instantiate(Affordance.TargetPrefab, _root.transform);
            }
            if (Affordance.LineRendererPrefab)
            {
                _lineRenderer = Object.Instantiate(Affordance.LineRendererPrefab, _root.transform);
            }

            if (GetOriginPosition(true, e, ref _originPosition))
            {
                _origin.transform.position = _originPosition;
            }
        }
        
        protected override void Teardown(TEvent e)
        {
            base.Teardown(e);
            if (_root)
            {
                // could be delayed for some kind of fade out effect
                Object.Destroy(_root);    
            }
        }
        
        protected override void Update(TEvent e)
        {
            base.Update(e);

            bool gotOrigin = GetOriginPosition(false, e, ref _originPosition);
            bool gotTarget = GetTargetPosition(false, e, _originPosition, ref _targetPosition);
            
            if (_origin && gotOrigin)
            {
                _origin.transform.position = _originPosition;
            }

            if (_target && gotTarget)
            {
                _target.transform.position = _targetPosition;
            }

            if (_lineRenderer && (gotOrigin || gotTarget))
            {
                _lineRendererPositions[0] = _originPosition;
                _lineRendererPositions[1] = _targetPosition;
                _lineRenderer.SetPositions(_lineRendererPositions);
            }
        }
    }
}