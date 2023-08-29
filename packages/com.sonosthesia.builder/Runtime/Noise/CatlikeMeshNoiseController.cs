using Unity.Jobs;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public abstract class CatlikeMeshNoiseController : MeshNoiseController
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

        [SerializeField] private Noise.Settings _noiseSettings = Noise.Settings.Default;

        [SerializeField] private int _seed;

        [SerializeField] private SpaceTRS _domain = new SpaceTRS { scale = 1f };

        protected sealed override JobHandle PerturbMesh(Mesh.MeshData meshData, int resolution, float displacement, JobHandle dependency)
        {
            return PerturbMesh(meshData, resolution, displacement, _noiseType, _dimensions, _noiseSettings, _seed, _domain, dependency);
        }

        protected abstract JobHandle PerturbMesh(
            Mesh.MeshData meshData, int resolution, float displacement,
            NoiseType noiseType, int dimensions, Noise.Settings settings, int seed, SpaceTRS domain,
            JobHandle dependency);

    }
}