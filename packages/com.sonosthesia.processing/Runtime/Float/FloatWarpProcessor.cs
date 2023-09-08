using System;
using UnityEngine;

namespace Sonosthesia.Processing
{
    [Serializable]
    public class FloatWarpSettings : DynamicProcessorSettings
    {
        [SerializeField] private bool _db;
        public bool Db => _db;

        [SerializeField] private float _scale = 1f;
        public float Scale => _scale;

        [SerializeField] private float _offset;
        public float Offset => _offset;
    }

    public class FloatWarpProcessor : DynamicProcessor<float, FloatWarpSettings>
    {
        public FloatWarpProcessor(FloatWarpSettings settings) : base(settings) { }

        protected override float Process(FloatWarpSettings settings, float input, float time)
        {
            if (settings.Db)
            {
                input = 10f * Mathf.Log10(input + 1f);
            }
            input = (input * settings.Scale) + settings.Offset;
            return input;
        }
    }
}