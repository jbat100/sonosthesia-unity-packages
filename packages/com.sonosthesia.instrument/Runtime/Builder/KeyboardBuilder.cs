using System;
using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class KeyboardBuilder : GroupInstantiator<KeyboardElement>
    {
        [Serializable]
        private class TransformProperties
        {
            [SerializeField] private Vector3 _scale;
            public Vector3 Scale => _scale;
            
            [SerializeField] private Vector3 _offset;
            public Vector3 Offset => _offset;
        }

        [Serializable]
        private class StateProperties
        {
            [SerializeField] private Material _material;
            public Material Material => _material;

            [SerializeField] private SingleUnityLayer _layer;
            public SingleUnityLayer Layer => _layer;
        }
        

        [SerializeField] private int _startNote;
        
        [SerializeField] private int _endNote;

        [SerializeField] private TransformProperties _whiteTransform;

        [SerializeField] private TransformProperties _blackTransform;
        
        [SerializeField] private StateProperties _whiteState;

        [SerializeField] private StateProperties _blackState;
        
        [SerializeField] private StateProperties _ghostState;

        [SerializeField] private List<MIDINoteFilter> _filters;
        
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

                StateProperties stateProperties = _filters.All(filter => filter.Allow(note)) ? (isWhite ? _whiteState : _blackState) : _ghostState;
                TransformProperties transformProperties = isWhite ? _whiteTransform : _blackTransform;

                Transform positionTarget = instance.transform;
                positionTarget.localPosition = transformProperties.Offset + offset * _spacing;

                Transform scaleTarget = instance.ScaleTarget ? instance.ScaleTarget : transform;
                scaleTarget.localScale = transformProperties.Scale;
                
                instance.Renderer.sharedMaterial = stateProperties.Material;
                instance.gameObject.SetLayerRecursively(stateProperties.Layer.LayerIndex);
            }
        }
    }
}