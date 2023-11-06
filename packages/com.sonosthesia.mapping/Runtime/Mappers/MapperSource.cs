using System;

namespace Sonosthesia.Mapping
{
    public class MapperSource<TValue> : MapperSlotSet<TValue> where TValue : struct
    {
        public IObservable<TValue> SlotObservable(string slotName)
        {
            return SlotSignal(slotName)?.SignalObservable;
        }
    }
}