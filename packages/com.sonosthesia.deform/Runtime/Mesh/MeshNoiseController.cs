using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public abstract class MeshNoiseController : DeformMeshController
    {
        protected enum NoiseType 
        {
            Perlin, PerlinSmoothTurbulence, PerlinValue, 
            Simplex, SimplexTurbulence, SimplexSmoothTurbulence, SimplexValue,
            VoronoiWorleyF1, VoronoiWorleyF2, VoronoiWorleyF2MinusF1, 
            VoronoiWorleySmoothLSE, VoronoiWorleySmoothPoly,
            VoronoiChebyshevF1, VoronoiChebyshevF2, VoronoiChebyshevF2MinusF1
        }
        
        [SerializeField] private NoiseType _noiseType;

        [SerializeField, Range(1, 3)] private int _dimensions = 1;

        protected sealed override JobHandle DeformMesh(UnityEngine.Mesh.MeshData meshData, int resolution, float displacement, JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, _noiseType, _dimensions, dependency);
        }
        
        protected abstract JobHandle PerturbMesh(
            UnityEngine.Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions,
            JobHandle dependency);
    }
}