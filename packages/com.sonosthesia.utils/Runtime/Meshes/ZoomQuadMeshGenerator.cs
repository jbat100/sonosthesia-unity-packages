using UnityEngine;

namespace Sonosthesia.Utils
{
    public class ZoomQuadMeshGenerator : MeshGenerator
    {
        [SerializeField] private Vector2 _xRange = Vector2.up;
        [SerializeField] private Vector2 _yRange = Vector2.up;
        [SerializeField] private Vector2 _uRange = Vector2.up;
        [SerializeField] private Vector2 _vRange= Vector2.up;
        
        protected override Mesh GenerateMesh()
        {
            return MeshGeneratorHelper.GenerateQuad(_xRange, _yRange, _uRange, _vRange);
        }
    }
}