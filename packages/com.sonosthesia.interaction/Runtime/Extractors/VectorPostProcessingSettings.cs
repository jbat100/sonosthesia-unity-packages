using System;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Interaction
{
    [Serializable]
    public class VectorPostProcessingSettings : IPostProcessing<Vector3>
    {
        [SerializeField] private VectorProcessingType _postProcessing;

        [SerializeField] private float _scale = 1f;
        
        public Vector3 PostProcess(Vector3 vector) => _postProcessing.ProcessVector(vector, _scale);
    }
}