using UnityEngine;

namespace Sonosthesia.Touch
{
    public class ValueTouchActor<TValue> : TouchActor where TValue : struct
    {
        // we use composition with ValueTouchEndpoint so that affordances can apply to both sources and actors
        
        [SerializeField] private TouchValueEventStreamContainer<TValue> _valueEventStreamContainer;
        public TouchValueEventStreamContainer<TValue> ValueEventStreamContainer => _valueEventStreamContainer;
    }
}