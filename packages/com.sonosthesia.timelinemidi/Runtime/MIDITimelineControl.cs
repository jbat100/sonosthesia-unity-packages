using UnityEngine;

namespace Sonosthesia.Timeline.MIDI
{
    // MIDI control class and related structs and enums

    #region Control parameter types

    public enum MIDINoteName {
        All, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B
    }

    public enum MIDIOctaveName {
        All, Minus2, Minus1, Zero, Plus1, Plus2, Plus3, Plus4, Plus5, Plus6, Plus7, Plus8
    }

    [System.Serializable]
    public struct MIDINoteFilter
    {
        public MIDINoteName noteName;
        public MIDIOctaveName octaveName;

        public bool Check(in MIDIEvent e)
        {
            return e.IsNote &&
                (octaveName == MIDIOctaveName.All || e.data1 / 12 == (int)octaveName - 1) &&
                (noteName   == MIDINoteName  .All || e.data1 % 12 == (int)noteName   - 1);
        }
    }

    [System.Serializable]
    public struct MidiEnvelope
    {
        // ADSR parameters
        public float attack;
        public float decay;
        public float sustain;
        public float release;

        // Times in seconds
        public float AttackTime  { get { return Mathf.Max(1e-5f, attack  / 10); } }
        public float DecayTime   { get { return Mathf.Max(1e-5f, decay   / 10); } }
        public float ReleaseTime { get { return Mathf.Max(1e-5f, release / 10); } }

        // Normalized sustain level value
        public float SustainLevel { get { return Mathf.Clamp01(sustain); } }
    }

    #endregion

    #region Serializable MIDI control class

    [System.Serializable]
    public sealed class MIDITimelineControl
    {
        // Is this control enabled?
        public bool enabled = true;

        // Control mode (Note/CC)
        public enum Mode { NoteEnvelope, NoteCurve, CC }
        public Mode mode = Mode.NoteEnvelope;

        // (Note mode) Note filter
        public MIDINoteFilter noteFilter = new MIDINoteFilter {
            noteName = MIDINoteName.All, octaveName = MIDIOctaveName.All
        };

        // (Note Envelope mode) Envelope parameters
        public MidiEnvelope envelope = new MidiEnvelope {
            attack = 0, decay = 1, sustain = 0.5f, release = 1
        };

        // (Note Curve mode) Envelope curve
        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0, 0, 90, 90),
            new Keyframe(0.02f, 1),
            new Keyframe(0.5f, 0)
        );

        // (CC mode) CC number
        public int ccNumber = 1;

        // Component/property options
        public ExposedReference<Component> targetComponent;
        public string propertyName;
        public string fieldName;

        // Value options
        public Vector4 vector0;
        public Vector4 vector1 = Vector3.one;
    }

    #endregion
}
