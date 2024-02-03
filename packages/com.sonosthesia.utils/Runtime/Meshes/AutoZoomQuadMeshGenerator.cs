using UnityEngine;

namespace Sonosthesia.Utils
{
    public class AutoZoomQuadMeshGenerator : MeshGenerator
    {
        [SerializeField] private Vector2 _size = Vector2.one;

        [SerializeField] private bool _centered = true;

        protected override Mesh GenerateMesh()
        {
            Vector2 xRange = Vector2.up * _size.x;
            Vector2 yRange = Vector2.up * _size.y;

            if (_centered)
            {
                xRange -= Vector2.one * (_size.x * 0.5f);
                yRange -= Vector2.one * (_size.y * 0.5f);
            }

            Vector2 uRange = Vector2.up;
            Vector2 vRange = Vector2.up;

            if (_size.x > _size.y)
            {
                float margin = (1 - _size.y / _size.x) * 0.5f;
                vRange = new Vector2(margin, 1 - margin);
                //Debug.Log($"{this} updated {nameof(vRange)} for {nameof(_size)} {_size},  {nameof(margin)} {margin}");
            }
            else if (_size.y < _size.x)
            {
                float margin = (1 - _size.x / _size.y) * 0.5f;
                uRange = new Vector2(margin, 1 - margin);
                //Debug.Log($"{this} updated {nameof(vRange)} for {nameof(_size)} {_size},  {nameof(margin)} {margin}");
            }
            
            return MeshGeneratorHelper.GenerateQuad(xRange, yRange, uRange, vRange);
        }
    }
}