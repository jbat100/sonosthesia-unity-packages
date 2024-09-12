using System;
using Sonosthesia.MIDI;
using Sonosthesia.Touch;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Instrument
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(MIDINoteRackTriggerSource), true), CanEditMultipleObjects]
    public class MIDINoteRackTriggerSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MIDINoteRackTriggerSource source = (MIDINoteRackTriggerSource)target;
            if(GUILayout.Button("Test Note"))
            {
                source.TestNote();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
#endif
    
    public class MIDINoteRackTriggerSource : TriggerSource<MIDINote>
    {
        [SerializeField] private string _rackElement;
        
        [SerializeField] private MIDIRackSetup _rackSetup;
        
        [SerializeField] private TriggerValueGenerator<float> _velocity;
        
        [SerializeField] private TriggerValueGenerator<float> _pressure;
        
        protected override bool Extract(bool initial, ITriggerData triggerData, out MIDINote midiNote)
        {
            midiNote = default;
            if (!_rackSetup.TryGet(_rackElement, out int channel, out int note))
            {
                return false;
            }
            if (!_velocity.ProcessTrigger(initial, triggerData, out float velocity))
            {
                return false;
            }
            // pressure is optional
            float pressure = 0;
            if (_pressure && !_pressure.ProcessTrigger(initial, triggerData, out  pressure))
            {
                return false;
            }
            midiNote = new MIDINote(channel, note, (int)velocity, (int)pressure);
            return true;
        }
        
        protected override void Clean(ITriggerData triggerData)
        {
            base.Clean(triggerData);
            _velocity.EndTrigger(triggerData);
            _pressure.EndTrigger(triggerData);
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