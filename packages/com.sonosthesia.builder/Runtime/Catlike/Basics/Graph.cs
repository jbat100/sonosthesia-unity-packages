using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform _pointPrefab;
        
        [SerializeField, Range(10, 200)] private int _resolution = 10;

        [SerializeField] private FunctionLibrary.FunctionName _functionName;
        
        [SerializeField, Min(0f)] private float _functionDuration = 2f;

        [SerializeField, Min(0f)] private float _transitionDuration = 1f;
        
        private bool _transitioning;
        private FunctionLibrary.FunctionName _transitionFunctionName;
        private float _currentDuration;
        private Transform[] _points;
        
        private FunctionLibrary.FunctionName GetNextFunctionName (FunctionLibrary.FunctionName name) 
        {
            return (FunctionLibrary.FunctionName )(((int)name + 1) % FunctionLibrary.Count());
        }
        
        protected void Awake ()
        {
            _points = new Transform[_resolution * _resolution];
            float step = 2f / _resolution;
            Vector3 scale = Vector3.one * step;
            for (int i = 0; i < _points.Length; i++) 
            {
                Transform point = _points[i] = Instantiate(_pointPrefab, transform, false);
                point.localScale = scale;
            }
        }

        protected void Update()
        {
            _currentDuration += Time.deltaTime;
            if (_transitioning)
            {
                if (_currentDuration >= _transitionDuration) 
                {
                    _currentDuration -= _transitionDuration;
                    _transitioning = false;
                }
            } 
            else if (_currentDuration >= _functionDuration) 
            {
                _currentDuration -= _functionDuration;
                _transitioning = true;
                _transitionFunctionName = _functionName;
                _functionName = GetNextFunctionName(_functionName);
            }
            
            if (_transitioning) 
            {
                UpdateFunctionTransition();
            }
            else {
                UpdateFunctionRegular();
            }
        }

        private void UpdateFunctionRegular() 
        {
            UpdateFunction(FunctionLibrary.GetFunction(_functionName));
        }
        
        private void UpdateFunctionTransition() 
        {
            FunctionLibrary.Function
                from = FunctionLibrary.GetFunction(_transitionFunctionName),
                to = FunctionLibrary.GetFunction(_functionName);
            float progress = _currentDuration / _transitionDuration;
            UpdateFunction((float u, float v, float t) => FunctionLibrary.Morph(u, v, t, from, to, progress));
        }
        
        private void UpdateFunction(FunctionLibrary.Function f)
        {
            float time = Time.time;
            float step = 2f / _resolution;
            float v = 0.5f * step - 1f;
            for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++) 
            {
                if (x == _resolution) 
                {
                    x = 0;
                    z += 1;
                    v = (z + 0.5f) * step - 1f;
                }
                float u = (x + 0.5f) * step - 1f;
                _points[i].localPosition = f(u, v, time);
            }
        }
        
        
    }
}