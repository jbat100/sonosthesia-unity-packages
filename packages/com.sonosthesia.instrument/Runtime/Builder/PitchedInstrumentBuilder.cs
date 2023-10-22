using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentBuilder : TransformedGroupInstantiator<PitchedInstrumentElement>
    {
        [Header("Pitch")]
        
        [SerializeField] private int _startNote;
        
        [SerializeField] private int _endNote;

        [SerializeField] private int _rootNote;

        [SerializeField] private ScaleDescriptor _scaleDescriptor;
        
        private enum OffsetSpread
        {
            Even,
            Interval
        }

        [Header("Spacing")] 
        
        [SerializeField] private OffsetSpread _offsetSpread;

        [SerializeField] private float _startOffset;

        [SerializeField] private float _endOffset = 1;

        private readonly MIDIScaleProvider _scaleProvider = new ();

        protected override void OnValidate()
        {
            _notes = null;
            base.OnValidate();
        }

        private IReadOnlyList<int> _notes;
        protected virtual IReadOnlyList<int> Notes
        {
            get
            {
                if (_notes != null)
                {
                    return _notes;
                }
                
                MIDIScale scale = _scaleProvider.GetScale(_scaleDescriptor);

                int octave = 0;

                // using local function to break out of nested loops
                List<int> Compute() 
                {
                    List<int> notes = new();
                    while (true)
                    {
                        foreach (int interval in scale.Intervals)
                        {
                            int note = _rootNote + (octave * 12) + interval;
                            if (note > _endNote)
                            {
                                return notes;
                            }
                            if (note > _startNote)
                            {
                                notes.Add(note);    
                            }
                        }
                        octave++;
                    }    
                }
                
                _notes = Compute().AsReadOnly();

                return _notes;
            }
        }

        protected override IReadOnlyList<float> ComputeOffsets()
        {
            IReadOnlyList<int> notes = Notes;
                
            List<float> ComputeEven()
            {
                List<float> offsets = new();
                int count = notes.Count;
                float offsetRange = _endOffset - _startOffset;
                float current = _startOffset;
                float increment = offsetRange / (count > 1 ? count - 1 : 1);
                for (int i = 0; i < count; i++)
                {
                    offsets.Add(current);
                    current += increment;
                }
                return offsets;
            }

            List<float> ComputeInterval()
            {
                List<float> offsets = new();
                foreach (int note in notes)
                {
                    offsets.Add(((float)note).Remap(_startNote, _endNote, _startOffset, _endOffset));
                }
                return offsets;
            }

            return _offsetSpread switch
            {
                OffsetSpread.Even => ComputeEven().AsReadOnly(),
                OffsetSpread.Interval => ComputeInterval().AsReadOnly(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void OnUpdatedInstances(IReadOnlyList<PitchedInstrumentElement> instances)
        {
            base.OnUpdatedInstances(instances);
            IReadOnlyList<int> notes = Notes;
            int count = Mathf.Min(instances.Count, notes.Count);
            for (int i = 0; i < count; i++)
            {
                instances[i].MIDINote = notes[i];
            }
        }
    }
}