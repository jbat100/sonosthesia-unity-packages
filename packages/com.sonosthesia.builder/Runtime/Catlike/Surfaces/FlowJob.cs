using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct FlowJob<N> : IJobParticleSystemParallelForBatch where N : struct, Noise.INoise
    {
        Noise.Settings settings;

        float3x4 domainTRS;

        float3x3 derivativeMatrix;

        float displacement;

        bool isPlane;
        
        NativeSlice<float4> GetSlice (NativeArray<float> data, int i) =>
            data.Slice(i, 4).SliceConvert<float4>();
        
        public void Execute(ParticleSystemJobData data, int startIndex, int count)
        {
            if (count != 4) 
            {
                return;
            }
            
            NativeSlice<float4>
                px = GetSlice(data.positions.x, startIndex),
                py = GetSlice(data.positions.y, startIndex),
                pz = GetSlice(data.positions.z, startIndex);
            
            NativeSlice<float4> life = GetSlice(data.aliveTimePercent, startIndex);
            
            float4x3 p = float4x3(px[0], 0f, pz[0]);
            
            life[0] = select(life[0], 100f, abs(p.c0) > 0.5f | abs(p.c2) > 0.5f);

            Noise.Sample4 noise = Noise.GetFractalNoise<N>(
                domainTRS.TransformVectors(p), settings
            ) * displacement;

            py[0] = noise.v + 0.05f;
        }
        
        public static JobHandle ScheduleParallel (
            ParticleSystem system,
            Noise.Settings settings, SpaceTRS domain, float displacement, bool isPlane
        ) => new FlowJob<N>() {
            settings = settings,
            domainTRS = domain.Matrix,
            derivativeMatrix = domain.DerivativeMatrix,
            displacement = displacement,
            isPlane = isPlane
        }.ScheduleBatch(system, 4);
    }
    
    public delegate JobHandle FlowJobScheduleDelegate (
        ParticleSystem system,
        Noise.Settings settings, SpaceTRS domain, float displacement, bool isPlane
    );
}