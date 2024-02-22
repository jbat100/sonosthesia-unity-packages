using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Sonosthesia.Noise;
using Unity.Mathematics;

namespace Sonosthesia.Deform
{
    public abstract class AdditiveDeformationComponent : MonoBehaviour
    {
        public virtual bool IsDynamic => false;
        
        /// <summary>
        /// Job based deformation computation given input mesh data
        /// </summary>
        /// <param name="meshData"></param>
        /// <param name="deformations"></param>
        /// <param name="innerloopBatchCount"></param>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public abstract JobHandle MeshDeformation(UnityEngine.Mesh.MeshData meshData, 
            NativeArray<Sample4> deformations, int innerloopBatchCount, JobHandle dependency);
        
        /// <summary>
        /// Single 4 wide compute of noise value and gradient
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public abstract Sample4 VertexDeformation(float3x4 vertex);
    }
}