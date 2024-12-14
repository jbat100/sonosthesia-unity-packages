using System;
using System.Collections.Generic;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "MIDIRackSetup", menuName = "Sonosthesia/Instrument/MIDIRackSetup")]
    public class MIDIRackSetup : ScriptableObject
    {
        [Serializable]
        private struct Element
        {
            [SerializeField] private MIDINoteName _note;
            public MIDINoteName Note => _note;
            
            [SerializeField, Range(-2, 8)] private int _octave;
            public int Octave => _octave;

            public bool TryGetNote(out int note)
            {
                return _note.MIDIPitchForOctave(_octave, out note);
            }
        }

        [Serializable]
        private struct NamedElement
        {
            [SerializeField] private string _name;
            public string Name => _name;

            [SerializeField] private Element _element;
            public Element Element => _element;
        }

        [SerializeField] private List<NamedElement> _elements = new ();

        private readonly Dictionary<string, Element> _elementMap = new ();

        protected virtual void OnValidate() => ReloadElementMap();

        protected virtual void OnEnable() => ReloadElementMap();

        private void ReloadElementMap()
        {
            _elementMap.Clear();
            foreach (NamedElement element in _elements)
            {
                _elementMap[element.Name] = element.Element;
            }
        }

        public bool TryGet(string elementName, out int note)
        {
            note = 0;
            return _elementMap.TryGetValue(elementName, out Element element) && element.TryGetNote(out note);
        }
    }
}