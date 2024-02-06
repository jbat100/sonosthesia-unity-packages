using Sonosthesia.AdaptiveMIDI.Messages;
using UnityEngine;

namespace Sonosthesia.MIDI
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
            Note = Mathf.Clamp(note, 0, 127);
            Velocity = Mathf.Clamp(velocity, 0, 127);
            Slide = Mathf.Clamp(slide, 0, 127);
            Pressure = Mathf.Clamp(pressure, 0, 127);
            Bend = Mathf.Clamp(bend, -48f, 48f);
        }

        public MPENote(MIDINoteOn note)
        {
            Note = note.Note;
            Velocity = note.Velocity;
            Slide = 0;
            Pressure = 0;
            Bend = 0;
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
        public static MIDINoteOn GetMIDINoteOn(this MPENote mpeNote, int channel)
        {
            return new MIDINoteOn(channel, mpeNote.Note, mpeNote.Velocity);
        }
        
        public static MIDINoteOff GetMIDINoteOff(this MPENote mpeNote, int channel)
        {
            return new MIDINoteOff(channel, mpeNote.Note, mpeNote.Velocity);
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
            const float scale = 8192f / MPENote.BEND_RANGE;
            //return new MIDIPitchBend(channel, 8192 + (int)(mpeNote.Bend * scale));
            return new MIDIPitchBend(channel, (int)(mpeNote.Bend * scale));
        }
    }
}