using System;
using UnityEngine;

namespace Sonosthesia.Flow
{
    [Serializable]
    public class WarpedCurve
    {
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _offset;
        [SerializeField] private float _scale = 1f;

        public float Evaluate(float time)
        {
            float curve = _curve.Evaluate(time);
            return _offset + curve * _scale;
        }
    }
    
    
    [Serializable]
    public class FloatModulation
    {
        [Tooltip("Curve time should be normalized")]
        [SerializeField] private WarpedCurve _curve;
        [SerializeField] private float _randomization;

        public float Modulate(float offset)
        {
            float curve = _curve.Evaluate(offset);
            float randomization = _randomization != 0 ? (UnityEngine.Random.value - 0.5f) * _randomization * 2f : 0f;
            return curve + randomization;
        }
    }

    [Serializable]
    public class ColorModulation
    {
        [SerializeField] private Gradient _gradient;
        [SerializeField] private FloatModulation _r;
        [SerializeField] private FloatModulation _g;
        [SerializeField] private FloatModulation _b;
        [SerializeField] private FloatModulation _a;
        
        public Color Modulate(float offset)
        {
            Color gradient = _gradient.Evaluate(offset);
            float r = Mathf.Clamp(_r.Modulate(offset), 0f, 1f);
            float g = Mathf.Clamp(_g.Modulate(offset), 0f, 1f);
            float b = Mathf.Clamp(_b.Modulate(offset), 0f, 1f);
            float a = Mathf.Clamp(_a.Modulate(offset), 0f, 1f);
            return new Color(r, g, b, a);
        }
    }
    
    [Serializable]
    public class VectorModulation
    {
        [SerializeField] private float _randomization;
        [SerializeField] private FloatModulation _x;
        [SerializeField] private FloatModulation _y;
        [SerializeField] private FloatModulation _z;
        
        public Vector3 Modulate(float offset)
        {
            Vector3 randomization = _randomization > 0 ? UnityEngine.Random.insideUnitSphere * _randomization : Vector3.zero;
            float x = _x.Modulate(offset);
            float y = _y.Modulate(offset);
            float z = _z.Modulate(offset);
            return new Vector3(x, y, z) + randomization;
        }
    }
    
    public abstract class Modulator<T> : MonoBehaviour where T : struct
    {
        public abstract T Modulate(T original, float offset);
    }
}