using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Sonosthesia.Processing;
using Sonosthesia.Signal;

namespace Sonosthesia.Mapping
{
    public class MapperConnection<TValue> : MapperConnectionBase where TValue : struct
    {
        [Serializable]
        public class Slot
        {
            public Slot()
            {
                
            }

            public Slot(string name)
            {
                _name = name;
            }
            
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private Signal<TValue> _signal;
            
            [SerializeField] private DynamicProcessorFactory<TValue> _processorFactory;

            public IDisposable Connect(Slot source, IDynamicProcessor<TValue> externalProcessor = null)
            {
                List<IDynamicProcessor<TValue>> processors = new List<IDynamicProcessor<TValue>>();
                if (externalProcessor != null)
                {
                    processors.Add(externalProcessor);
                }
                if (source._processorFactory)
                {
                    processors.Add(source._processorFactory.Make());
                }
                if (_processorFactory)
                {
                    processors.Add(_processorFactory.Make());
                }

                if (processors.Count == 0)
                {
                    return source._signal.SignalObservable
                        .Subscribe(value => _signal.Broadcast(value));
                }

                float startTime = Time.time;
                ProcessorChain<TValue> chain = new ProcessorChain<TValue>(processors.ToArray());
                return source._signal.SignalObservable
                    .Subscribe(value => _signal.Broadcast(chain.Process(value, Time.time - startTime)));
            }
        }

        [SerializeField] private List<Slot> _slots;
        public IEnumerable<string> SlotNames => _slots.Select(slot => slot.Name);

        [SerializeField] private List<string> _compatibility;
        public IEnumerable<string> Compatibility => _compatibility.AsReadOnly();

        public Slot GetSlot(string slotName)
        {
            return _slots.FirstOrDefault(slot => slot.Name == name);
        }

        public override bool HasSlot(string slotName)
        {
            return _slots.Any(slot => slot.Name == slotName);
        }

        public override void CreateSlot(string slotName)
        {
            _slots.Add(new Slot(slotName));
        }

        public override void DeleteSlot(string slotName)
        {
            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                if (_slots[i] != null && _slots[i].Name == slotName)
                {
                    _slots.RemoveAt(i);
                }
            }
        }

        public override void DeleteAllSlots()
        {
            _slots.Clear();
        }
    }
}