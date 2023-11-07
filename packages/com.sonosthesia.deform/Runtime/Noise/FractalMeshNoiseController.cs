using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public abstract class FractalMeshNoiseController : CatlikeMeshNoiseController
    {
        [SerializeField] private Noise.Settings _noiseSettings = Noise.Settings.Default;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        protected sealed override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, int seed,
            JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, noiseType, dimensions, _noiseSettings, seed, _domain, dependency);
        }

        protected abstract JobHandle PerturbMesh(
            Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain,
            JobHandle dependency);
        
    }
}