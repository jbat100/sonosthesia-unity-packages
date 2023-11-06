namespace Sonosthesia.Mapping
{
    public class MapperTarget<TValue> : MapperSlotSet<TValue> where TValue : struct
    {
        public void Broadcast(string slotName, TValue value)
        {
            SlotSignal(slotName)?.Broadcast(value);
        }
    }
}