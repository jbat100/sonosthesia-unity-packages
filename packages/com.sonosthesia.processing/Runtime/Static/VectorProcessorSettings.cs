using System;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class VectorProcessorSettings : IProcessor<Vector3>
    {
        [SerializeField] private VectorProcessingType _processor;

        [SerializeField] private float _scale = 1f;
        
        public Vector3 Process(Vector3 vector) => _processor.ProcessVector(vector, _scale);
    }
}