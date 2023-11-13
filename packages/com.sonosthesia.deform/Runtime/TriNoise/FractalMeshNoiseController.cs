using Sonosthesia.Noise;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public abstract class FractalMeshNoiseController : CatlikeMeshNoiseController
    {
        [SerializeField] private FractalSettings _noiseSettings = FractalSettings.Default;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        protected sealed override JobHandle PerturbMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, int seed,
            JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, noiseType, dimensions, _noiseSettings, seed, _domain, dependency);
        }

        protected abstract JobHandle PerturbMesh(
            UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, FractalSettings settings, int seed, SpaceTRS domain,
            JobHandle dependency);
        
    }
}