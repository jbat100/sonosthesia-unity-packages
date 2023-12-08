using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Sonosthesia.Timeline.MIDI
{
    // Custom inspector for MIDI assets
    // There is no editable property; It just shows some infomation.
    [CustomEditor(typeof(MIDIAnimationAsset))]
    class MIDIAnimationAssetEditor : Editor
    {
        string _tempoText;
        string _durationText;
        string _noteText;
        string _ccText;
        string _pressureText;

        void OnEnable()
        {
            var asset = ((MIDIAnimationAsset)target).template;

            _tempoText = asset.tempo.ToString();

            var bars = (float)asset.duration / (asset.ticksPerQuarterNote * 4);
            _durationText = bars.ToString() + (bars > 1 ? " bars" : " bar");

            var note = new HashSet<int>();
            var cc = new HashSet<int>();
            var pressure = new HashSet<int>();

            foreach (var e in asset.events)
            {
                switch (e.status & 0xf0u)
                {
                    case 0x80u:
                    case 0x81u: note.Add(e.data1); break;
                    case 0xb0u: cc.Add(e.data1); break;
                    case 0xa0u: pressure.Add(e.data1); break;
                }
            }

            _noteText = note.Count == 0 ? "-" : string.Join(", ", note.OrderBy(x => x));
            _ccText = cc.Count == 0 ? "-" : string.Join(", ", cc.OrderBy(x => x));
            _pressureText = cc.Count == 0 ? "-" : string.Join(", ", pressure.OrderBy(x => x));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Tempo", _tempoText);
            EditorGUILayout.LabelField("Duration", _durationText);
            EditorGUILayout.LabelField("Contained Events");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Note", _noteText);
            EditorGUILayout.LabelField("CC", _ccText);
            EditorGUILayout.LabelField("Pressure", _pressureText);
            EditorGUI.indentLevel--;
        }
    }
}
