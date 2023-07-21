using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform _pointPrefab;
        
        [SerializeField, Range(10, 100)] int _resolution = 10;

        [SerializeField] private FunctionLibrary.FunctionName _functionName;
        
        private Transform[] _points;
        
        protected void Awake ()
        {
            _points = new Transform[_resolution * _resolution];
            float step = 2f / _resolution;
            Vector3 position = Vector3.zero;
            Vector3 scale = Vector3.one * step;
            for (int i = 0, x = 0, z = 0; i < _points.Length; i++, x++) 
            {
                if (x == _resolution)
                {
                    x = 0;
                    z += 1;
                }
                Transform point = _points[i] = Instantiate(_pointPrefab, transform, false);
                position.x = (x + 0.5f) * step - 1f;
                position.z = (z + 0.5f) * step - 1f;
                point.localPosition = position;
                point.localScale = scale;
            }
        }

        protected void Update()
        {
            float time = Time.time;
            for (int i = 0; i < _points.Length; i++)
            {
                Transform point = _points[i];
                Vector3 position = point.localPosition;
                position.y = FunctionLibrary.GetFunction(_functionName)(position.x, position.z, time);
                point.localPosition = position;
            }
        }
    }
}