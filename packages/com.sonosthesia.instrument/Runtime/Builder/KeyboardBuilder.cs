using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class KeyboardBuilder : GroupInstantiator<KeyboardElement>
    {
        [Serializable]
        private class KeyProperties
        {
            [SerializeField] private Vector3 _scale;
            public Vector3 Scale => _scale;
            
            [SerializeField] private Vector3 _offset;
            public Vector3 Offset => _offset;
            
            [SerializeField] private Material _material;
            public Material Material => _material;
        }

        [SerializeField] private int _startNote;
        
        [SerializeField] private int _endNote;

        [SerializeField] private KeyProperties _white;

        [SerializeField] private KeyProperties _black;

        [SerializeField] private Vector3 _spacing = Vector3.right;
        
        protected override int RequiredCount => Mathf.Max(_endNote - _startNote + 1, 0);
        
        private static bool NoteIsWhite(int note)
        {
            int octaveNote = note % 12;
            switch (octaveNote)
            {
                case 1:
                case 3:
                case 6:
                case 8:
                case 10:
                    return false;
                default:
                    return true;
            }
        }
        
        protected override void OnUpdatedInstances(IReadOnlyList<KeyboardElement> instances)
        {
            base.OnUpdatedInstances(instances);
            
            float offset = 0f;
            for (int i = 0; i < instances.Count; i++)
            {
                int note = _startNote + i;
                KeyboardElement instance = instances[i];
                instance.MIDINote = note;
                
                bool isWhite = NoteIsWhite(note);
                offset += isWhite && NoteIsWhite(note - 1) ? 1f : 0.5f;
                
                KeyProperties properties = isWhite ? _white : _black;
                Transform t = instance.transform;
                t.localScale = properties.Scale;
                t.localPosition = properties.Offset + offset * _spacing;

                instance.Renderer.sharedMaterial = properties.Material;
            }
        }
    }
}