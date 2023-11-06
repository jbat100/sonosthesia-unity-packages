using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sonosthesia.Processing;
using Sonosthesia.Signal;

namespace Sonosthesia.Mapping
{
    public class Mapper<TValue> : ScriptableObject where TValue : struct
    {
        [Serializable] 
        public class Connection
        {
            [SerializeField] private string _source;
            public string Source => _source;
            
            [SerializeField] private string _target;
            public string Target => _target;
            
            [SerializeField] private DynamicProcessorFactory<TValue> _processorFactory;
            public DynamicProcessorFactory<TValue> ProcessorFactory => _processorFactory;
        }
        
        [SerializeField] private bool _autoMap;

        [SerializeField] private List<Connection> _connections;

        [SerializeField] private List<string> _compatibility;
        public IEnumerable<string> Compatibility => _compatibility.AsReadOnly();

        public IDisposable Map(MapperSource<TValue> source, MapperTarget<TValue> target)
        {
            CompositeDisposable subscriptions = new CompositeDisposable();

            HashSet<string> targetSlots = new HashSet<string>(target.SlotNames);

            foreach (Connection connection in _connections)
            {
                // we don't want to pipe multiple sources into a given target slot
                if (!targetSlots.Contains(connection.Target))
                {
                    continue;
                }
                
                IObservable<TValue> sourceObservable = source.SlotObservable(connection.Source);
                Signal<TValue> targetSignal = target.SlotSignal(connection.Target);   
                
                if (sourceObservable == null || !targetSignal)
                {
                    continue;
                }

                targetSlots.Remove(connection.Target);

                IDynamicProcessor<TValue> processor = connection.ProcessorFactory.Make();
                float startTime = Time.time;

                subscriptions.Add(sourceObservable.Subscribe(value =>
                {
                    if (processor != null)
                    {
                        value = processor.Process(value, Time.time - startTime);
                    }
                    targetSignal.Broadcast(value);
                }));
            }
            
            if (_autoMap)
            {
                // attempt to map the remaining target slots with source slots with the same name
                foreach (string targetSlot in targetSlots)
                {
                    IObservable<TValue> sourceObservable = source.SlotObservable(targetSlot);
                    Signal<TValue> targetSignal = target.SlotSignal(targetSlot);   
                    
                    if (sourceObservable == null || !targetSignal)
                    {
                        continue;
                    }
                    
                    subscriptions.Add(sourceObservable.Subscribe(value =>
                    {
                        targetSignal.Broadcast(value);
                    }));
                }
            }
            
            return subscriptions;
        }
    }
}