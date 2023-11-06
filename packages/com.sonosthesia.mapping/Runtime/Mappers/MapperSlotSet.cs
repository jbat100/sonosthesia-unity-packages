using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Mapping
{
    public class MapperSlotSet<TValue> : MonoBehaviour where TValue : struct
    {
        [Serializable]
        public class Slot
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private Signal<TValue> _signal;
            public Signal<TValue> Signal => _signal;
        }

        [SerializeField] private List<Slot> _slots;

        [SerializeField] private List<string> _compatibility;
        public IEnumerable<string> Compatibility => _compatibility.AsReadOnly();

        public IEnumerable<string> SlotNames => _slots.Select(slot => slot.Name);

        internal Signal<TValue> SlotSignal(string slotName)
        {
            return _slots.FirstOrDefault(slot => slot.Name == slotName)?.Signal;
        }
    }
}