using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI.Messages
{
    public readonly struct MPENote
    {
        // 48 semitones in each up/down direction is used by Ableton Live
        public const float BEND_RANGE = 48f;
        
        public readonly int Note;
        public readonly int Velocity;
        public readonly int Slide;
        public readonly int Pressure;
        public readonly float Bend; // in semitones
        
        public MPENote(int note, int velocity, int slide, int pressure, float bend)
        {
            Note = note;
            Velocity = velocity;
            Slide = slide;
            Pressure = pressure;
            Bend = bend;
        }

        public MPENote ChangeSlide(int diff)
        {
            return new MPENote(Note, Velocity, Mathf.Clamp(Slide + diff, 0, 127), Pressure, Bend);
        }
        
        public MPENote ChangePressure(int diff)
        {
            return new MPENote(Note, Velocity, Slide, Mathf.Clamp(Pressure + diff, 0, 127), Bend);
        }
        
        public MPENote ChangeBend(float diff)
        {
            return new MPENote(Note, Velocity, Slide, Pressure, Mathf.Clamp(Bend + diff, -BEND_RANGE, BEND_RANGE));
        }
        
        public override string ToString()
        {
            return $"{nameof(MPENote)} <{nameof(Note)} {Note} {nameof(Velocity)} {Velocity} {nameof(Slide)} {Slide} " +
                   $"{nameof(Pressure)} {Pressure} {nameof(Bend)} {Bend}>";
        }
    }

    public static class MPENoteExtensions
    {
        public static MIDINote GetMIDINoteOn(this MPENote mpeNote, int channel)
        {
            return new MIDINote(channel, mpeNote.Note, mpeNote.Velocity, mpeNote.Pressure);
        }
        
        public static MIDINote GetMIDINoteOff(this MPENote mpeNote, int channel)
        {
            return new MIDINote(channel, mpeNote.Note, mpeNote.Velocity, mpeNote.Pressure);
        }

        public static MIDIControl GetSlideControl(this MPENote mpeNote, int channel)
        {
            return new MIDIControl(channel, 74, mpeNote.Slide);
        }
        
        public static MIDIChannelAftertouch GetChannelAftertouch(this MPENote mpeNote, int channel)
        {
            return new MIDIChannelAftertouch(channel, mpeNote.Pressure);
        }
        
        public static MIDIPitchBend GetPitchBend(this MPENote mpeNote, int channel)
        {
            const float scale = 16383f / (2 * MPENote.BEND_RANGE);
            return new MIDIPitchBend(channel, (int)(mpeNote.Bend * scale));
        }
    }
}