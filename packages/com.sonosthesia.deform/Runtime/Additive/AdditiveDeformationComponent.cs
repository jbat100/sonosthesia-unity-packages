using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Sonosthesia.Noise;

namespace Sonosthesia.Deform
{
    public abstract class AdditiveDeformationComponent : MonoBehaviour
    {
        public virtual bool IsDynamic => false;
        
        public abstract JobHandle GetDeformation(UnityEngine.Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, int innerloopBatchCount, JobHandle dependency);
    }
}