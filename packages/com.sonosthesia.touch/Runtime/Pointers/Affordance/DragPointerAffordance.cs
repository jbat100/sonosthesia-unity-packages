using System;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class DragPointerAffordance : AgnosticPointerAffordance
    {
        // TODO : use pools 

        [SerializeField] private Transform _originPrefab;
        
        [SerializeField] private Transform _targetPrefab;
            
        [SerializeField] private LineRenderer _lineRendererPrefab;   

        [SerializeField] private float _offset = 0.1f;

        protected class Controller : IObserver<PointerSourceEvent>
        {
            private readonly DragPointerAffordance _affordance;

            private bool _initialized;
            private Guid _eventId;
            private GameObject _root;
            private Transform _origin;
            private Transform _target;
            private LineRenderer _lineRenderer;

            private Vector3 _originPosition;
            private Camera _camera;
            private readonly Vector3[] _lineRendererPositions = new Vector3[2];

            private void CheckInitialized(PointerSourceEvent value)
            {
                if (_initialized)
                {
                    return;
                }
                _initialized = true;
                _eventId = value.Id;
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

                _camera = Camera.main;
                Vector3 cameraPosition = _camera.transform.position;
                Vector3 look = value.Data.pointerCurrentRaycast.worldPosition - cameraPosition;
                float distance = look.magnitude;
                float offsetDistance = Mathf.Max(_camera.nearClipPlane, distance - _affordance._offset);
                
                _originPosition = cameraPosition + look * (offsetDistance / distance);
            }
            
            public Controller(DragPointerAffordance affordance)
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

            public void OnNext(PointerSourceEvent value)
            {
                CheckInitialized(value);

                Vector3 direction = _camera.transform.position - _originPosition;

                //Plane plane = new Plane(direction.normalized, _originPosition);
                
                Plane plane = new Plane(-_affordance.transform.forward, _originPosition);
                
                Ray ray = _camera.ScreenPointToRay(value.Data.position);

                if (!plane.Raycast(ray, out float enter))
                {
                    return;
                }
                
                Vector3 targetPosition = ray.GetPoint(enter);

                if (_origin)
                {
                    _origin.transform.position = _originPosition;
                }

                if (_target)
                {
                    _target.transform.position = targetPosition;
                }

                if (_lineRenderer)
                {
                    _lineRendererPositions[0] = _originPosition;
                    _lineRendererPositions[1] = targetPosition;
                    _lineRenderer.SetPositions(_lineRendererPositions);
                }
            }
        }

        protected override IObserver<PointerSourceEvent> MakeController()
        {
            return new Controller(this);
        }

        
    }
}