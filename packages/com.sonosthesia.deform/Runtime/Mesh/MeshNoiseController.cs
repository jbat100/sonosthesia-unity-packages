using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public abstract class MeshNoiseController : DeformMeshController
    {
        [SerializeField] private CatlikeNoiseType _noiseType;

        [SerializeField, Range(1, 3)] private int _dimensions = 1;

        protected sealed override JobHandle DeformMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, _noiseType, _dimensions, dependency);
        }
        
        protected abstract JobHandle PerturbMesh(
            UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            CatlikeNoiseType noiseType, int dimensions,
            JobHandle dependency);
    }
}