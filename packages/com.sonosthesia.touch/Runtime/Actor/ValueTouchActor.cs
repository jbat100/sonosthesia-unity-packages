using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Touch
{
    public class ValueTouchActor<TValue> : TouchActor where TValue : struct
    {
        // we use composition with ValueTouchEndpoint so that affordances can apply to both sources and actors
        
        [SerializeField] private TouchValueEventChannel<TValue> _valueEventChannel;
        public TouchValueEventChannel<TValue> ValueEventChannel => _valueEventChannel;
    }
}