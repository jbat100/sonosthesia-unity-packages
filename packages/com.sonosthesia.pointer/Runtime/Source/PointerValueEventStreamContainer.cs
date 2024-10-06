using System;
using UnityEngine.EventSystems;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Pointer
{
    // used for affordances
    public readonly struct PointerValueEvent<TValue> : IValueEvent<TValue> where TValue : struct
    {
        public readonly Guid Id;
        public readonly TValue Value;
        public readonly PointerEventData Data;

        public PointerValueEvent(Guid id, TValue value, PointerEventData data)
        {
            Id = id;
            Value = value;
            Data = data;
        }

        public TValue GetValue() => Value;
    }
    
    public class PointerValueEventStreamContainer<TValue> : ValueEventStreamContainer<TValue, PointerValueEvent<TValue>>
        where TValue : struct
    {
        
    }
}