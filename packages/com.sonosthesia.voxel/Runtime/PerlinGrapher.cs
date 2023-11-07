using UnityEngine;

namespace Sonosthesia.Builder
{
    
    [ExecuteInEditMode]
    public class PerlinGrapher : MonoBehaviour
    {
        [SerializeField] private PerlinSettings settings;
        
        private LineRenderer _lineRenderer;
        public LineRenderer LineRenderer => _lineRenderer;

        protected void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 100;
            Graph();
            settings.OnChanged.AddListener(Graph);
        }

        protected void OnDestroy() => settings.OnChanged.RemoveListener(Graph);

        protected void OnValidate() => Graph();

        private void Graph()
        {
            if (!_lineRenderer)
            {
                return;
            }
            
            int z = 11;
            Vector3[] positions = new Vector3[_lineRenderer.positionCount];
            for (int x = 0; x < _lineRenderer.positionCount; x++)
            {
                float y = VoxelUtils.FBM2(x, z, settings);
                positions[x] = new Vector3(x, y, z);
            }
            _lineRenderer.SetPositions(positions);
        }
    }
}