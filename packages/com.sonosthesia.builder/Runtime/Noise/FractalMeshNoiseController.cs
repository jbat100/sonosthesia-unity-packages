using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public abstract class FractalMeshNoiseController : CatlikeMeshNoiseController
    {
        [SerializeField] private Noise.Settings _noiseSettings = Noise.Settings.Default;

        protected sealed override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, int seed, SpaceTRS domain,
            JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, noiseType, dimensions, _noiseSettings, seed, domain, dependency);
        }

        protected abstract JobHandle PerturbMesh(
            Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain,
            JobHandle dependency);
        
    }
}