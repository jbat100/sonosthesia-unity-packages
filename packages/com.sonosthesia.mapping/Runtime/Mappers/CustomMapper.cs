using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using Sonosthesia.Processing;

namespace Sonosthesia.Mapping
{
    public class CustomMapper<TValue> : Mapper<TValue> where TValue : struct
    {

        [Serializable]
        public class ConnectionInfo
        {
            [SerializeField] private string _source;
            public string Source => _source;
            
            [SerializeField] private string _target;
            public string Target => _target;
            
            [SerializeField] private DynamicProcessorFactory<TValue> _processorFactory;
            public DynamicProcessorFactory<TValue> ProcessorFactory => _processorFactory;
        }
        
        [SerializeField] private bool _autoMap;

        [SerializeField] private List<ConnectionInfo> _connections;

        [SerializeField] private List<string> _compatibility;
        public IEnumerable<string> Compatibility => _compatibility.AsReadOnly();

        public override IDisposable Map(MapperConnection<TValue> source, MapperConnection<TValue> target)
        {
            CompositeDisposable subscriptions = new CompositeDisposable();

            HashSet<string> targetSlotNames = new HashSet<string>(target.SlotNames);

            foreach (ConnectionInfo connectionInfo in _connections)
            {
                // we don't want to pipe multiple sources into a given target slot
                if (!targetSlotNames.Contains(connectionInfo.Target))
                {
                    continue;
                }
                
                MapperConnection<TValue>.Slot targetSlot = target.GetSlot(connectionInfo.Target);
                MapperConnection<TValue>.Slot sourceSlot = source.GetSlot(connectionInfo.Source);
                
                if (targetSlot == null || sourceSlot == null)
                {
                    continue;
                }

                targetSlotNames.Remove(connectionInfo.Target);
                IDynamicProcessor<TValue> processor = connectionInfo.ProcessorFactory ? connectionInfo.ProcessorFactory.Make() : null;
                subscriptions.Add(targetSlot.Connect(sourceSlot, processor));
            }
            
            if (_autoMap)
            {
                // attempt to map the remaining target slots with source slots with the same name
                foreach (string targetSlotName in targetSlotNames.ToList())
                {
                    MapperConnection<TValue>.Slot targetSlot = target.GetSlot(targetSlotName);
                    MapperConnection<TValue>.Slot sourceSlot = source.GetSlot(targetSlotName); 
                    
                    if (targetSlot == null || sourceSlot == null)
                    {
                        continue;
                    }

                    targetSlotNames.Remove(targetSlotName);
                    subscriptions.Add(targetSlot.Connect(sourceSlot));
                }
            }

            if (targetSlotNames.Count > 0)
            {
                Debug.LogWarning($"{this} target slots are not filled : {string.Join(", ", targetSlotNames)}");
            }
            
            return subscriptions;
        }
    }
}