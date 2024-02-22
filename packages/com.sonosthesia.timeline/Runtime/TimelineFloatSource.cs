using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Timeline
{
    public class TimelineFloatSource : MonoBehaviour
    {
        [SerializeField] private float _value;

        [SerializeField] private Signal<float> _target;

        protected virtual void Update()
        {
            _target.Broadcast(_value);
        }
    }    
}


