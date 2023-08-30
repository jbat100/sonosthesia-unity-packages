using System.Runtime.CompilerServices;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public static class MathExtensions 
    {
        public static float4x3 TransformVectors (this float3x4 trs, float4x3 p, float w = 1f) => float4x3(
            trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
            trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
            trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
        );
        
        public static float4x3 TransformVectors (this float3x3 m, float4x3 v) => float4x3(
            m.c0.x * v.c0 + m.c1.x * v.c1 + m.c2.x * v.c2,
            m.c0.y * v.c0 + m.c1.y * v.c1 + m.c2.y * v.c2,
            m.c0.z * v.c0 + m.c1.z * v.c1 + m.c2.z * v.c2
        );
        
        public static float3x4 Get3x4 (this float4x4 m) => float3x4(m.c0.xyz, m.c1.xyz, m.c2.xyz, m.c3.xyz);
        
        public static float4x3 NormalizeRows (this float4x3 m) 
        {
            float4 normalizer = rsqrt(m.c0 * m.c0 + m.c1 * m.c1 + m.c2 * m.c2);
            return float4x3(m.c0 * normalizer, m.c1 * normalizer, m.c2 * normalizer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 Smoothstep(this float4 value)
        {
            return value * value * (3f - 2f * value);
        }
        
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
        public static Sample4 GetFractalNoise<N>(this float4x3 position, Noise.Settings settings, 
            int seed, float displacement, float3x3 derivativeMatrix)
            where N : struct, Noise.INoise
        {
            Sample4 result = Noise.GetFractalNoise<N>(position, settings, seed) * displacement;
            result.Derivatives = derivativeMatrix.TransformVectors(result.Derivatives);
            return result;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this float4x3 position, Noise.Settings settings, 
            TriNoise.NoisePhase phase, float3x3 derivativeMatrix)
            where N : struct, Noise.INoise
        {
            return position.GetFractalNoise<N>(settings, phase.Seed, phase.Displacement, derivativeMatrix);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Sample4 GetFractalNoise<N>(this float4x3 position, Noise.Settings settings, TriNoise.TriPhase triPhase, float3x3 derivativeMatrix)
            where N : struct, Noise.INoise
        {
            Sample4 noise1 = position.GetFractalNoise<N>(settings, triPhase.C1, derivativeMatrix);
            Sample4 noise2 = position.GetFractalNoise<N>(settings, triPhase.C2, derivativeMatrix);
            Sample4 noise3 = position.GetFractalNoise<N>(settings, triPhase.C3, derivativeMatrix);
            return noise1 + noise2 + noise3;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this float4x3 position, Noise.Settings settings, 
            int seed, float displacement) where N : struct, Noise.ISimpleNoise
        {
            return Noise.GetSimpleFractalNoise<N>(position, settings, seed) * displacement;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this float4x3 position, Noise.Settings settings, 
            TriNoise.NoisePhase phase) where N : struct, Noise.ISimpleNoise
        {
            return position.GetSimpleFractalNoise<N>(settings, phase.Seed, phase.Displacement);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 GetSimpleFractalNoise<N>(this float4x3 position, Noise.Settings settings, 
            TriNoise.TriPhase triPhase) where N : struct, Noise.ISimpleNoise
        {
            float4 noise1 = position.GetSimpleFractalNoise<N>(settings, triPhase.C1);
            float4 noise2 = position.GetSimpleFractalNoise<N>(settings, triPhase.C2);
            float4 noise3 = position.GetSimpleFractalNoise<N>(settings, triPhase.C3);
            return noise1 + noise2 + noise3;
        }
        
        
    }
}