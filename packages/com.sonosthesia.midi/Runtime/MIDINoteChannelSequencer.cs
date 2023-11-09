using System;
using Sonosthesia.AdaptiveMIDI.Messages;
using Sonosthesia.Sequencer;
using UnityEngine;

namespace Sonosthesia.MIDI
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MIDINoteChannelSequencer))]
    public class MIDINoteChannelSequencerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MIDINoteChannelSequencer sequencer = (MIDINoteChannelSequencer)target;
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
    public class MIDINoteChannelSequenceElement : ChannelSequenceElement
    {
        [SerializeField] private int _note;
        public int Note => _note;

        [SerializeField] private int _velocity;
        public int Velocity => _velocity;
    }

    public class MIDINoteChannelSequencer : ChannelSequencer<MIDINote, MIDINoteChannelSequenceElement>
    {
        [SerializeField] private int _channel;

        protected override MIDINote ForgeStart(MIDINoteChannelSequenceElement element)
        {
            return new MIDINote(_channel, element.Note, element.Velocity);
        }

        protected override MIDINote ForgeEnd(MIDINoteChannelSequenceElement element)
        {
            return new MIDINote(_channel, element.Note, 0);
        }
    }
}