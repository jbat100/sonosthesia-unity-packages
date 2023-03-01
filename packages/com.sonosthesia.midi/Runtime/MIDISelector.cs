using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.MIDI
{
    public class MIDISelector : Selector<MIDINote>
    {
        private enum Selection
        {
            None,
            Unit,
            Channel,
            Note,
            Velocity
        }

        [SerializeField] private Selection _selection;

        protected override float InternalSelect(MIDINote value)
        {
            return _selection switch
            {
                Selection.None => 0f,
                Selection.Unit => 1f,
                Selection.Channel => value.Channel / 16f,
                Selection.Note => value.Note / 127f,
                Selection.Velocity => value.Velocity  / 127f,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}