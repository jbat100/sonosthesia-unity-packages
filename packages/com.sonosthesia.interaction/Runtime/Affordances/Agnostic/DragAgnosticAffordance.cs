using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class DragAgnosticAffordance<TEvent, TStreamContainer, TAffordance> : AgnosticAffordance<TEvent, TStreamContainer, TAffordance> 
        where TEvent : struct 
        where TStreamContainer : MonoBehaviour, IStreamContainer<TEvent>
        where TAffordance : DragAgnosticAffordance<TEvent, TStreamContainer, TAffordance> 
    {
        // TODO : use pools 

        [SerializeField] private Transform _originPrefab;
        
        [SerializeField] private Transform _targetPrefab;
        
        [SerializeField] private LineRenderer _lineRendererPrefab;
        
        protected new abstract class Controller : AgnosticAffordance<TEvent, TStreamContainer, TAffordance>.Controller
        {
            private GameObject _root;
            private Transform _origin;
            private Transform _target;
            private LineRenderer _lineRenderer;

            private Vector3 _originPosition;
            private Vector3 _targetPosition;
            
            private Camera _camera;
            private readonly Vector3[] _lineRendererPositions = new Vector3[2];
            
            public Controller(Guid eventId, TAffordance affordance) : base(eventId, affordance)
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
                if (Affordance._originPrefab)
                {
                    _origin = Instantiate(Affordance._originPrefab, _root.transform);
                }
                if (Affordance._targetPrefab)
                {
                    _target = Instantiate(Affordance._targetPrefab, _root.transform);
                }
                if (Affordance._lineRendererPrefab)
                {
                    _lineRenderer = Instantiate(Affordance._lineRendererPrefab, _root.transform);
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
                    Destroy(_root);    
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
}