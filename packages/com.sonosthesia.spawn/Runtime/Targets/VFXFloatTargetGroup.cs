using System;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Spawn
{
    [Serializable]
    public class VFXFloatTargetGroupLink : VFXTargetGroupLink<float>
    {
        [SerializeField] private float _offset;

        [SerializeField] private float _scale;

        protected override float InternalMap(float source)
        {
            return _scale * source + _offset;
        }
    }

    public class VFXFloatTargetGroup : VFXTargetGroup<float, VFXFloatTargetGroupLink>
    {
        protected override void Apply(VisualEffect visualEffect, string key, float value)
        {
            visualEffect.SetFloat(key, value);
        }
    }
}