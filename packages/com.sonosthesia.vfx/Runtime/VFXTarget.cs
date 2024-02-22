using UnityEngine;
using UnityEngine.VFX;
using Sonosthesia.Signal;

namespace Sonosthesia.VFX
{
    public abstract class VFXTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private string _nameID;

        protected override void Awake()
        {
            base.Awake();
            if (!_visualEffect)
            {
                _visualEffect = GetComponent<VisualEffect>();
            }
        }

        protected sealed override void Apply(T value)
        {
            Apply(value, _visualEffect, _nameID);
        }

        protected abstract void Apply(T value, VisualEffect visualEffect, string nameID);
    }
}