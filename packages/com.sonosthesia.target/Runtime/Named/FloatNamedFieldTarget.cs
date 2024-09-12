using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Target
{
    public class FloatNamedFieldTarget : NamedFieldTarget<float>
    {
        [SerializeField] private FloatProcessor _postProcessor;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
    }
}