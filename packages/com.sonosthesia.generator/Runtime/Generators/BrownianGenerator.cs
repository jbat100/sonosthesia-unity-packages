using UnityEngine;

namespace Sonosthesia.Generator
{
    public abstract class BrownianGenerator<T> : Generator<T> where T : struct
    {
        [SerializeField] private float _amount = 10f;
        protected float Amount => _amount;
        
        [SerializeField] private int _octaves = 2;
        protected int Octaves => _octaves;
        
        [SerializeField] private uint _seed = 0;

        protected abstract void Rehash(uint seed);

        protected virtual void Awake() => Rehash(_seed);
    }
}