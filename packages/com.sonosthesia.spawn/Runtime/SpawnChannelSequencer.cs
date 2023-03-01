using System;
using Sonosthesia.Flow;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Spawn
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(SpawnChannelSequencer))]
    public class SpawnChannelSequencerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpawnChannelSequencer sequencer = (SpawnChannelSequencer)target;
            if(GUILayout.Button("Play"))
            {
                sequencer.Play();
            }
            if(GUILayout.Button("Stop"))
            {
                sequencer.Stop();
            }
        }
    }
#endif
    
    [Serializable]
    public class SpawnChannelSequenceElement : ChannelSequenceElement
    {
        [SerializeField] private Vector3 _position;
        [SerializeField] private Vector3 _rotation;
        public RigidTransform Trans => new (Quaternion.Euler(_rotation), _position);

        [SerializeField] private float _size;
        public float Size => _size;
        
        [SerializeField] private float _lifetime;
        public float Lifetime => _lifetime;
        
        [SerializeField] private Color _color;
        public Color Color => _color;
    }

    public class SpawnChannelSequencer : ChannelSequencer<SpawnPayload, SpawnChannelSequenceElement>
    {
        protected override SpawnPayload ForgeStart(SpawnChannelSequenceElement element)
        {
            return new SpawnPayload(element.Trans, element.Size, element.Lifetime, element.Color);
        }

        protected override SpawnPayload ForgeEnd(SpawnChannelSequenceElement element)
        {
            return ForgeStart(element);
        }
    }
}