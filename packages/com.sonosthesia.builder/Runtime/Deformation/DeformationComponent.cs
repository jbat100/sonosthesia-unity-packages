using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public abstract class DeformationComponent : MonoBehaviour
    {
        public abstract JobHandle GetDeformation(NativeArray<float3x4> positions, NativeArray<Sample4> deformations);
    }
}