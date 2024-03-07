using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Touch
{
    public class LineProximityAffordance : ProximityAffordance
    {
        [SerializeField] private LineRenderer _lineRenderer;

        [SerializeField] private Vector3 _offset;

        [SerializeField] private float _fade;

        [SerializeField] private AnimationCurve _distanceWidth;
        
        [SerializeField] private AnimationCurve _distanceIntensity;

        [SerializeField] private Color _color;

        [SerializeField] private Color _emissionColor;
        
        [SerializeField] private string _intensityProperty;
        
        [SerializeField] private string _emissionColorProperty;

        private Vector3 _lastTarget;
        private float _alpha;
        private bool _running;
        private int? _intensityPropertyID = null;
        private int? _emissionColorPropertyID = null;

        protected virtual void OnEnable()
        {
            if (!string.IsNullOrEmpty(_intensityProperty) && _lineRenderer.material.HasFloat(_intensityProperty))
            {
                _intensityPropertyID = Shader.PropertyToID(_intensityProperty);    
            }

            if (!string.IsNullOrEmpty(_emissionColorProperty) && _lineRenderer.material.HasColor(_emissionColorProperty))
            {
                _emissionColorPropertyID = Shader.PropertyToID(_emissionColorProperty);   
            }
            _running = false;
            _alpha = 0;
        }

        protected virtual void Update()
        {
            Vector3 point = transform.TransformPoint(_offset);
            if (ClosestZone(point, out ProximityZone zone, out Vector3 target, out float distance))
            {
                _lastTarget = target;
                _running = true;
            }
            else
            {
                _running = false;
            }

            float maxAlphaChange = _fade == 0 ? float.PositiveInfinity : Time.deltaTime / _fade;
            float alphaTarget = _running ? 1 : 0;

            _alpha = Mathf.MoveTowards(_alpha, alphaTarget, maxAlphaChange);

            if (_alpha < 1e-6)
            {
                _lineRenderer.enabled = false;
            }
            else
            {
                float width = _distanceWidth.Evaluate(distance);
                float intensity = _distanceIntensity.Evaluate(distance);
                
                _lineRenderer.SetPosition(0, point);
                _lineRenderer.SetPosition(1, _lastTarget);
                
                _lineRenderer.startWidth = width;
                _lineRenderer.endWidth = width;

                _lineRenderer.startColor = _color;
                _lineRenderer.endColor = _color;
                
                _lineRenderer.enabled = true;
                
                if (_intensityPropertyID.HasValue)
                {
                    _lineRenderer.material.SetFloat(_intensityPropertyID.Value, intensity);   
                }

                if (_emissionColorPropertyID.HasValue)
                {
                    Color emissionColor = HDRColor.Make(_emissionColor, intensity);
                    _lineRenderer.material.SetColor(_emissionColorPropertyID.Value, emissionColor);
                }
            }

        }

    }
}