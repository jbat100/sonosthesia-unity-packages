using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Sonosthesia.Noise;

namespace Sonosthesia.Deform
{
    public abstract class DeformationComponent : MonoBehaviour
    {
        public abstract JobHandle GetDeformation(NativeArray<float3x4> positions, 
            NativeArray<Sample4> deformations, SpaceTRS domain, float lerp, int innerloopBatchCount, JobHandle dependency);
        
        public abstract JobHandle GetDeformation(UnityEngine.Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, SpaceTRS domain, float lerp, int innerloopBatchCount, JobHandle dependency);
    }
}