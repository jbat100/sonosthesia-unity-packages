using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public class MonoMeshNoiseController : FractalMeshNoiseController
    {
        protected override JobHandle PerturbMesh(
            Mesh.MeshData meshData, int resolution, float displacement, 
            NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain,
            JobHandle dependency)
        {
            return SurfaceJob.Jobs[(int) noiseType, dimensions - 1](
                meshData,
                resolution,
                settings,
                seed,
                domain,
                displacement,
                IsPlane,
                dependency);
        }
        
    }
}