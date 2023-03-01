using Sonosthesia.Flow;
using UnityEngine;
using UnityEngine.VFX;

namespace Sonosthesia.Spawn
{
    public abstract class VFXTarget<T> : Target<T> where T : struct
    {
        [SerializeField] private VisualEffect _visualEffect;

        [SerializeField] private string _nameID;
        
        protected sealed override void Apply(T value)
        {
            Apply(value, _visualEffect, _nameID);
        }

        protected abstract void Apply(T value, VisualEffect visualEffect, string nameID);
    }
}