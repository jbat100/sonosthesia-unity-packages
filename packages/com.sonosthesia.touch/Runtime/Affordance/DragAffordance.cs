using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DragAffordance<TEvent, TSource, TAffordance> : AgnosticAffordance<TEvent, TSource> 
        where TEvent : struct 
        where TSource : MonoBehaviour, IStreamSource<TEvent>
        where TAffordance : DragAffordance<TEvent, TSource, TAffordance> 
    {
        // TODO : use pools 

        [SerializeField] private Transform _originPrefab;
        
        [SerializeField] private Transform _targetPrefab;
        
        [SerializeField] private LineRenderer _lineRendererPrefab; 

        protected abstract class Controller : IObserver<TEvent>
        {
            private readonly TAffordance _affordance;
            protected TAffordance Affordance => _affordance;

            private bool _initialized;
            private Guid _eventId;
            private GameObject _root;
            private Transform _origin;
            private Transform _target;
            private LineRenderer _lineRenderer;

            private Vector3 _originPosition;
            private Vector3 _targetPosition;
            
            private Camera _camera;
            private readonly Vector3[] _lineRendererPositions = new Vector3[2];

            protected abstract bool GetOriginPosition(bool initial, TEvent value, ref Vector3 origin);
            protected abstract bool GetTargetPosition(bool initial, TEvent value, Vector3 origin, ref Vector3 target);

            protected abstract bool GetOriginScale(bool initial, TEvent value, ref Vector3 origin);
            
            protected abstract bool GetTargetScale(bool initial, TEvent value, Vector3 origin, ref Vector3 target);
            
            private void CheckInitialized(TEvent value)
            {
                if (_initialized)
                {
                    return;
                }
                _initialized = true;
                _root = new GameObject(_eventId.ToString());
                _root.transform.SetParent(_affordance.transform);
                if (_affordance._originPrefab)
                {
                    _origin = Instantiate(_affordance._originPrefab, _root.transform);
                }
                if (_affordance._targetPrefab)
                {
                    _target = Instantiate(_affordance._targetPrefab, _root.transform);
                }
                if (_affordance._lineRendererPrefab)
                {
                    _lineRenderer = Instantiate(_affordance._lineRendererPrefab, _root.transform);
                }

                if (GetOriginPosition(true, value, ref _originPosition))
                {
                    _origin.transform.position = _originPosition;
                }
            }

            protected virtual void Update(TEvent updatedEvent)
            {
                
            }
            
            public Controller(Guid id, TAffordance affordance)
            {
                _affordance = affordance;
            }
            
            public void OnCompleted()
            {
                if (_root)
                {
                    // could be delayed for some kind of fade out effect
                    Destroy(_root);    
                }
            }

            public void OnError(Exception error)
            {
                if (_root)
                {
                    // could be delayed for some kind of fade out effect
                    Destroy(_root);    
                }
            }

            public void OnNext(TEvent value)
            {
                CheckInitialized(value);
                
                Update(value);

                bool gotOrigin = GetOriginPosition(false, value, ref _originPosition);
                bool gotTarget = GetTargetPosition(false, value, _originPosition, ref _targetPosition);
                
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