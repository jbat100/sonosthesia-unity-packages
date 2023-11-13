using System.Runtime.CompilerServices;
using Sonosthesia.Noise;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    public static class MathExtensions 
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetNoise<N>(this float4x3 position, TriNoise.NoisePhase phase, int frequency) 
            where N : struct, Noise.INoise
        {
            SmallXXHash4 hash = SmallXXHash4.Seed(phase.Seed);
            return default(N).GetNoise4(position, hash, frequency) * phase.Displacement;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetNoise<N>(this float4x3 position, TriNoise.NoisePhase phase, int frequency, float3x3 derivativeMatrix) 
            where N : struct, Noise.INoise
        {
            Sample4 result = position.GetNoise<N>(phase, frequency);
            result.Derivatives = derivativeMatrix.TransformVectors(result.Derivatives);
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetNoise<N>(this float4x3 position, TriNoise.NoiseComponent config, float3x3 derivativeMatrix) 
            where N : struct, Noise.INoise
        {
            Sample4 noise1 = position.GetNoise<N>(config.TriPhase.C1, config.Frequency, derivativeMatrix);
            Sample4 noise2 = position.GetNoise<N>(config.TriPhase.C2, config.Frequency, derivativeMatrix);
            Sample4 noise3 = position.GetNoise<N>(config.TriPhase.C3, config.Frequency, derivativeMatrix);
            return noise1 + noise2 + noise3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleNoise<N>(this float4x3 position, TriNoise.NoisePhase phase, int frequency) 
            where N : struct, Noise.ISimpleNoise
        {
            SmallXXHash4 hash = SmallXXHash4.Seed(phase.Seed);
            return default(N).GetNoiseValue4(position, hash, frequency) * phase.Displacement;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleNoise<N>(this float4x3 position, TriNoise.NoiseComponent config) 
            where N : struct, Noise.ISimpleNoise
        {
            float4 noise1 = position.GetSimpleNoise<N>(config.TriPhase.C1, config.Frequency);
            float4 noise2 = position.GetSimpleNoise<N>(config.TriPhase.C2, config.Frequency);
            float4 noise3 = position.GetSimpleNoise<N>(config.TriPhase.C3, config.Frequency);
            return noise1 + noise2 + noise3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this float4x3 position, FractalSettings settings, 
            TriNoise.NoisePhase phase, float3x3 derivativeMatrix)
            where N : struct, Noise.INoise
        {
            return position.GetFractalNoise<N>(settings, phase.Seed, phase.Displacement, derivativeMatrix);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this float4x3 position, FractalSettings settings, TriNoise.TriPhase triPhase, float3x3 derivativeMatrix)
            where N : struct, Noise.INoise
        {
            Sample4 noise1 = position.GetFractalNoise<N>(settings, triPhase.C1, derivativeMatrix);
            Sample4 noise2 = position.GetFractalNoise<N>(settings, triPhase.C2, derivativeMatrix);
            Sample4 noise3 = position.GetFractalNoise<N>(settings, triPhase.C3, derivativeMatrix);
            return noise1 + noise2 + noise3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this float4x3 position, FractalSettings settings, 
            TriNoise.NoisePhase phase) where N : struct, Noise.ISimpleNoise
        {
            return position.GetSimpleFractalNoise<N>(settings, phase.Seed, phase.Displacement);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this float4x3 position, FractalSettings settings, 
            TriNoise.TriPhase triPhase) where N : struct, Noise.ISimpleNoise
        {
            float4 noise1 = position.GetSimpleFractalNoise<N>(settings, triPhase.C1);
            float4 noise2 = position.GetSimpleFractalNoise<N>(settings, triPhase.C2);
            float4 noise3 = position.GetSimpleFractalNoise<N>(settings, triPhase.C3);
            return noise1 + noise2 + noise3;
        }
        
        
    }
}