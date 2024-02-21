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

            public Slot(string name, Signal<TValue> signal)
            {
                _name = name;
                _signal = signal;
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
                        .Subscribe(value =>
                        {
                            _signal.Broadcast(value);
                        });
                }

                float startTime = Time.time;
                ProcessorChain<TValue> chain = new ProcessorChain<TValue>(processors.ToArray());
                return source._signal.SignalObservable
                    .Subscribe(value =>
                    {
                        _signal.Broadcast(chain.Process(value, Time.time - startTime));
                    });
            }
        }

        // note : initializing _slots avoids some occasional null ref errors when using Autofill/Clear outside play mode
        
        [SerializeField] private List<Slot> _slots = new();
        public IEnumerable<string> SlotNames => _slots.Select(slot => slot.Name);

        [SerializeField] private List<string> _compatibility;
        public IEnumerable<string> Compatibility => _compatibility.AsReadOnly();

        public Slot GetSlot(string slotName)
        {
            return _slots.FirstOrDefault(slot => slot.Name == slotName);
        }

#if UNITY_EDITOR
        public override void AutofillSlots(bool recursive)
        {
            FillSlots(transform, recursive);
        }

        private void FillSlots(Transform root, bool recursive)
        {
            foreach (Transform child in root)
            {
                if (_slots.All(slot => slot.Name != child.name))
                {
                    Signal<TValue> signal = child.GetComponent<Signal<TValue>>();
                    if (signal)
                    {
                        _slots.Add(new Slot(child.name, signal));
                    }
                }
                if (recursive)
                {
                    FillSlots(child, true);
                }
            }
        }

        public override void DeleteAllSlots()
        {
            _slots.Clear();
        }
#endif
    }
}