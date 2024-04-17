using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline
{
    public class AnimationFloatSignalSource : MonoBehaviour
    {
        [SerializeField] private float _value;

        [SerializeField] private Signal<float> _target;

        protected virtual void Update()
        {
            _target.Broadcast(_value);
        }
    }    
}


