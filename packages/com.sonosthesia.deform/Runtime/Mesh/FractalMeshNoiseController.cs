using Sonosthesia.Noise;
using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public abstract class FractalMeshNoiseController : MeshNoiseController
    {
        [SerializeField] private FractalNoiseSettings _noiseSettings = FractalNoiseSettings.Default;
        
        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };
        
        [SerializeField] private int _seed;
        
        protected sealed override JobHandle PerturbMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            CatlikeNoiseType noiseType, int dimensions,
            JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, noiseType, dimensions, _noiseSettings, _seed, _domain, dependency);
        }

        protected abstract JobHandle PerturbMesh(
            UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            CatlikeNoiseType noiseType, int dimensions, FractalNoiseSettings settings, int seed, SpaceTRS domain,
            JobHandle dependency);
        
    }
}