using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDINoteSelector : Selector<MIDINote>
    {
        private enum Selection
        {
            None,
            Unit,
            Channel,
            Note,
            Velocity,
            Pressure
        }

        [SerializeField] private Selection _selection;

        [SerializeField] private bool _normalize;

        protected override float InternalSelect(MIDINote value)
        {
            return _selection switch
            {
                Selection.Unit => 1f,
                Selection.Channel => _normalize ? value.Channel / 16f : value.Channel,
                Selection.Note => _normalize ? value.Note / 127f : value.Note,
                Selection.Velocity => _normalize ? value.Velocity  / 127f : value.Velocity,
                Selection.Pressure => _normalize ? value.Pressure  / 127f : value.Pressure,
                _ => 0f
            };
        }
    }
}