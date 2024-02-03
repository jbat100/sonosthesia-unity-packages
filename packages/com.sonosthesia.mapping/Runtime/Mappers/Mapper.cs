using System;
using Sonosthesia.Processing;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Mapping
{
    public static class MapperUtils
    {
        public static IDisposable AutoMap<TValue>(MapperConnection<TValue> source, MapperConnection<TValue> target)
            where TValue: struct
        {
            CompositeDisposable subscriptions = new CompositeDisposable();

            // attempt to map the remaining target slots with source slots with the same name
            foreach (string targetSlotName in target.SlotNames)
            {
                subscriptions.Add(Connect(targetSlotName, source, target));
            }

            return subscriptions;
        }

        public static IDisposable Connect<TValue>(string slotName,
            MapperConnection<TValue> source, MapperConnection<TValue> target,
            IDynamicProcessor<TValue> processor = null) where TValue: struct
        {
            MapperConnection<TValue>.Slot targetSlot = target.GetSlot(slotName);
            MapperConnection<TValue>.Slot sourceSlot = source.GetSlot(slotName);
            if (targetSlot == null || sourceSlot == null)
            {
                return Disposable.Empty;
            }
            return targetSlot.Connect(sourceSlot, processor);
        }
    }
    
    public abstract class Mapper<TValue> : ScriptableObject where TValue : struct
    {
        public abstract IDisposable Map(MapperConnection<TValue> source, MapperConnection<TValue> target);
    }
}