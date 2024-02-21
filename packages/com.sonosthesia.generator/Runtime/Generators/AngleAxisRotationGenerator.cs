using UnityEngine;

namespace Sonosthesia.Generator
{
    public class AngleAxisRotationGenerator : Generator<Quaternion>
    {
        [SerializeField] private Vector3 _axis = Vector3.up;
        
        [SerializeField] private float _offset;
        
        // do not specify frequency, use GeneratorSignal timeFactor instead so that it keeps the rotation 
        // smooth, spins 1 revolution per unit time
        
        public override Quaternion Evaluate(float time)
        {
            return Quaternion.AngleAxis((time + _offset) * 360f, _axis);
        }
    }
}