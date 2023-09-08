using UnityEngine;

namespace Sonosthesia.Utils
{
    public class SoftLanding
    {
        public float Speed { get; set; }
        
        public float Target { get; set; }
        
        public float Current { get; private set; }
        
        public float Step(float deltaTime)
        {
            Current = Current > Target ? 
                Mathf.MoveTowards(Current, Target, Time.deltaTime * Speed) : 
                Target;
            return Current;
        }
        
    }
}