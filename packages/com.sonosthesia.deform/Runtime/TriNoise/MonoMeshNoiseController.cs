using Sonosthesia.Noise;
using Unity.Jobs;

namespace Sonosthesia.Deform
{
    public class MonoMeshNoiseController : FractalMeshNoiseController
    {
        protected override JobHandle PerturbMesh(
            UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, 
            NoiseType noiseType, int dimensions, FractalSettings settings, int seed, SpaceTRS domain,
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