using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Generator
{
    public class FloatGeneratorSignal : GeneratorSignal<float>
    {
        [SerializeField] private FloatProcessor _postProcessor;

        protected override float PostProcess(float value) => _postProcessor.Process(value);
    }
}