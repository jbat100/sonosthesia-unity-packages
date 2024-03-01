using System;
using UnityEngine;
using UnityEngine.VFX;
using Sonosthesia.Signal;

namespace Sonosthesia.VFX
{
    public abstract class VFXTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private string _nameID;

        private int _intNameID;

        protected override void Awake()
        {
            base.Awake();
            if (!_visualEffect)
            {
                _visualEffect = GetComponent<VisualEffect>();
            }

            _intNameID = Shader.PropertyToID(_nameID);
        }

        protected virtual void OnValidate()
        {
            _intNameID = Shader.PropertyToID(_nameID);
        }

        protected sealed override void Apply(T value)
        {
            Apply(value, _visualEffect, _intNameID);
        }

        protected abstract void Apply(T value, VisualEffect visualEffect, int nameID);
    }
}