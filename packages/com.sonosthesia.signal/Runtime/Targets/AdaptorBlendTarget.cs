using UnityEngine;

namespace Sonosthesia.Flow
{
    // this is a bit silly, used to handle cases like TransformDirectionTarget, TransformSizeTarget
    // where the blended type is not the 
    // same as the signal type, probably best to try to avoid this situation using Adaptors or visual scripting
    
    public abstract class AdaptorBlendTarget<T, A, B> : Target<T> where T : struct where A : struct where B : struct, IBlender<A>
    {
        [SerializeField] private TargetBlend _targetBlend;

        private A _reference;

        private B _blender;

        protected abstract A Reference { get; }

        protected abstract A Adapt(T value);

        protected abstract void ApplyBlended(A value);
        
        protected override void Awake()
        {
            _reference = Reference;
            _blender = default;
        }

        protected sealed override void Apply(T value)
        {
            A blended = _blender.Blend(_targetBlend, _reference, Adapt(value));
            ApplyBlended(blended);
        }
    }
}