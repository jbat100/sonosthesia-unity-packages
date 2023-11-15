using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Deform
{
    public abstract class NoiseDeformableSplineExtrude : DeformableSplineExtrude
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

        [SerializeField, Range(1, 3)] private int _dimensions = 3;
        
        [SerializeField] private int _seed;

        protected sealed override void Deform(ISpline spline, UnityEngine.Mesh.MeshData data, 
            float radius, int sides, float segmentsPerUnit, bool capped, float2 range)
        {
            Deform(spline, data, radius, sides, segmentsPerUnit, capped, range, _noiseType, _dimensions, _seed);
        }

        protected abstract void Deform(ISpline spline, UnityEngine.Mesh.MeshData data,
            float radius, int sides, float segmentsPerUnit, bool capped, float2 range,
            NoiseType noiseType, int dimensions, int seed);
    }
}