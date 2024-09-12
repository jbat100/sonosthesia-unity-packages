using UnityEngine;
using UnityEngine.VFX;
using Sonosthesia.Target;

namespace Sonosthesia.VFX
{
    public abstract class VFXTarget<T, B> : BlendTarget<T, B> where T : struct where B : struct, IBlender<T>
    {
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private string _nameID;

        private int _intNameID;

        protected sealed override T Reference => ExtractReference(_visualEffect, _intNameID);

        protected override void Awake()
        {
            _intNameID = Shader.PropertyToID(_nameID);
            if (!_visualEffect)
            {
                _visualEffect = GetComponent<VisualEffect>();
            }
            // note : base order call is important
            base.Awake();
        }

        protected override void OnEnable()
        {
            _intNameID = Shader.PropertyToID(_nameID);
            base.OnEnable();
        }
        
        protected virtual void OnValidate()
        {
            _intNameID = Shader.PropertyToID(_nameID);
        }

        protected sealed override void ApplyBlended(T value)
        {
            Apply(value, _visualEffect, _intNameID);
        }

        protected abstract void Apply(T value, VisualEffect visualEffect, int nameID);

        protected abstract T ExtractReference(VisualEffect visualEffect, int nameID);
    }
}