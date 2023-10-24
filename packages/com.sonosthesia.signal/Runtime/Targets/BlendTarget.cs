using UnityEngine;

namespace Sonosthesia.Flow
{
    public enum TargetBlend
    {
        None,
        Override,
        Add
    }

    public interface IBlender<T> where T : struct
    {
        T Blend(TargetBlend blend, T reference, T value);
    }

    public struct FloatBlender : IBlender<float>
    {
        public float Blend(TargetBlend blend, float reference, float value) => blend switch
        {
            TargetBlend.Override => value,
            TargetBlend.Add => reference + value,
            _ => 0f
        };
    }
    
    public struct Vector3Blender : IBlender<Vector3>
    {
        public Vector3 Blend(TargetBlend blend, Vector3 reference, Vector3 value) => blend switch
        {
            TargetBlend.Override => value,
            TargetBlend.Add => reference + value,
            _ => Vector3.zero
        };
    }
    
    public struct QuaternionBlender : IBlender<Quaternion>
    {
        public Quaternion Blend(TargetBlend blend, Quaternion reference, Quaternion value) => blend switch
        {
            TargetBlend.Override => value,
            TargetBlend.Add => reference * value,
            _ => Quaternion.identity
        };
    }

    public abstract class BlendTarget<T, B> : Target<T> where T : struct where B : struct, IBlender<T>
    {
        [SerializeField] private TargetBlend _targetBlend;

        private T _reference;

        private B _blender;

        protected abstract T Reference { get; }

        protected abstract void ApplyBlended(T value);
        
        protected override void Awake()
        {
            _reference = Reference;
            _blender = default;
        }

        protected sealed override void Apply(T value)
        {
            T blended = _blender.Blend(_targetBlend, _reference, value);
            ApplyBlended(blended);
        }
    }
}