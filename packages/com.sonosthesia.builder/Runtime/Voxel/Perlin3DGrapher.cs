using UnityEngine;

namespace Sonosthesia.Builder
{
    public class Perlin3DGrapher : MonoBehaviour
    {
        [SerializeField] private Vector3 _dimensions = new Vector3(10, 10, 10);
        
        [SerializeField] private PerlinSettings settings;

        [SerializeField] [Range(0, 10)] private float _drawCutoff;

        private void CreateCubes()
        {
            for (int z = 0; z < _dimensions.z; z++)
            {
                for (int y = 0; y < _dimensions.y; y++)
                {
                    for (int x = 0; x < _dimensions.x; x++)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.name = $"Perlin_{x}_{y}_{z}";
                        cube.transform.SetParent(transform);
                        cube.transform.position = new Vector3(x, y, z);
                    }
                }
            }
        }

        protected void OnValidate() => Graph();

        private void Graph()
        {
            // destroy existing cubes
            MeshRenderer[] cubes = GetComponentsInChildren<MeshRenderer>();
            
            if (cubes.Length == 0)
                CreateCubes();

            // deal with editor weirdness
            if (cubes.Length == 0)
                return;

            for (int z = 0; z < _dimensions.z; z++)
            {
                for (int y = 0; y < _dimensions.y; y++)
                {
                    for (int x = 0; x < _dimensions.x; x++)
                    {
                        float noise = VoxelUtils.FBM3(x, y, z, settings);
                        MeshRenderer meshRenderer = cubes.FlatGet((int) _dimensions.x, (int) _dimensions.z, x, y, z);
                        meshRenderer.enabled = noise > _drawCutoff;
                    }
                }
            }
        }
        
    }
}