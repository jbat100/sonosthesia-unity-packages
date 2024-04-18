using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Scaffold;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class PitchedInstrumentBuilder : TransformedGroupInstantiator<PitchedInstrumentElement>
    {
        private enum OffsetSpread
        {
            Even,
            Interval
        }
        
        [Header("Pitch")]
        
        [SerializeField] private int _startNote;
        
        [SerializeField] private int _endNote;

        [SerializeField] private List<MIDINoteFilter> _filters;

        [Header("Appearance")] 
        
        [SerializeField] private OffsetSpread _offsetSpread;

        [SerializeField] private float _startOffset;

        [SerializeField] private float _endOffset = 1;

        [SerializeField] private MIDINoteMaterialProvider _materialProvider;
        
        protected override IObservable<Unit> RefreshRequestObservable =>
            _filters.Select(f => f.ChangeObservable).Merge();

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

                // using local function to break out of nested loops
                List<int> Compute() 
                {
                    List<int> notes = new();
                    int startNote = Mathf.Max(0, _startNote);
                    int endNote = Mathf.Min(127, _endNote);
                    for (int i = startNote; i <= endNote; i++)
                    {
                        int note = i;
                        if (_filters.All(filter => filter.Allow(note)))
                        {
                            notes.Add(note);   
                        }
                    }

                    return notes;
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
                int note = notes[i];
                PitchedInstrumentElement element = instances[i];
                element.MIDINote = note;
                Renderer elementRenderer = element.Renderer;
                Material elementMaterial = _materialProvider ? _materialProvider.MaterialForNote(note) : null;
                if (elementRenderer && elementMaterial)
                {
                    elementRenderer.sharedMaterial = elementMaterial;
                }
            }
        }
    }
}