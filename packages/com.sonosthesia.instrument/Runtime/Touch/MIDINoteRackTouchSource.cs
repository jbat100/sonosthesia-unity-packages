using System;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Instrument
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MIDINoteRackTouchSource), true), CanEditMultipleObjects]
    public class MIDINoteRackTouchSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MIDINoteRackTouchSource source = (MIDINoteRackTouchSource)target;
            if(GUILayout.Button("Test Note"))
            {
                source.TestNote();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
#endif
    
    public class MIDINoteRackTouchSource : ValueTouchSource<MIDINote>
    {
        [SerializeField] private string _rackElement;
        
        [SerializeField] private MIDIRackSetup _rackSetup;
        
        [SerializeField] private TouchValueGenerator<float> _velocity;
        
        [SerializeField] private TouchValueGenerator<float> _pressure;
        
        protected override bool Extract(bool initial, ITouchData touchData, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_rackSetup.TryGet(_rackElement, out int channel, out int note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, touchData, out float velocity))
            {
                return false;
            }
            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTrigger(initial, touchData, out  pressure))
            {
                return false;
            }
            midiNote = new MIDINote(channel, note, (int)velocity, (int)pressure);
            return true;
        }
        
        protected override void CleanupStream(Guid id, ITouchData touchData)
        {
            base.CleanupStream(id, touchData);
            _velocity.EndTouch(touchData);
            _pressure.EndTouch(touchData);
        }

        public void TestNote()
        {
            if (!_rackSetup.TryGet(_rackElement, out int channel, out int note))
            {
                return;
            }
            MIDINote midiNote = new MIDINote(channel, note, 120, 120);
            TestStream(midiNote);
        }
    }
}