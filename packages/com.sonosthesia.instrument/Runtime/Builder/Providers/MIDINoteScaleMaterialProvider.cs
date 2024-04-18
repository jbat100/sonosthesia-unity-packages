using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class MIDINoteScaleMaterialProvider : MIDINoteMaterialProvider
    {
        [Tooltip("Reference, ignored if filter is set")]
        [SerializeField] private MIDINoteName _reference;
        
        [Tooltip("Filter providing reference note")]
        [SerializeField] private MIDIScaleNoteFilter _filter;

        [SerializeField] private Material _defaultMaterial;

        [Serializable]
        public class Choice
        {
            [SerializeField] private MIDIInterval _interval;
            public MIDIInterval Interval => _interval;

            [SerializeField] private Material _material;
            public Material Material => _material;
        }

        [SerializeField] private List<Choice> _materials;

        public override Material MaterialForNote(int note)
        {
            MIDINoteName noteName = note.ToMIDINoteName();
            MIDINoteName reference = _filter ? _filter.Note : _reference;
            MIDIInterval interval = MIDIIntervalUtils.NoteUpwardInterval(reference, noteName);
            Material result = _materials.FirstOrDefault(c => c.Interval == interval)?.Material;
            return result ? result : _defaultMaterial;
        }
    }
}