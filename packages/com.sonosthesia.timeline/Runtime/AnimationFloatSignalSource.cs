using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Timeline
{
    public class AnimationFloatSignalSource : MonoBehaviour
    {
        [SerializeField] private float _value;

        [SerializeField] private InterfaceReference<ISignal<float>> _target;

        protected virtual void Update()
        {
            _target.Value?.Broadcast(_value);
        }
    }    
}


