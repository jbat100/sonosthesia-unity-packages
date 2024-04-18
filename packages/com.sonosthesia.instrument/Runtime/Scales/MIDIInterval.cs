using System;
using Sonosthesia.MIDI;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    // intervals castable to int semitones
    public enum MIDIInterval
    {
        Root = 0,
        MinorSecond = 1,
        MajorSecond = 2,
        MinorThird = 3,
        MajorThird = 4,
        Fourth = 5,
        TriTone = 6,
        Fifth = 7,
        MinorSixth = 8,
        MajorSixth = 9,
        MinorSeventh = 10,
        MajorSeventh = 11
    }

    public static class MIDIIntervalUtils
    {
        public static MIDIInterval NoteUpwardInterval(MIDINoteName first, MIDINoteName second)
        {
            int firstNote = (int) first;
            int secondNote = (int) second;
            int difference = ModFloor(secondNote - firstNote, 12);
            return (MIDIInterval)(difference);
        }

        public static MIDIInterval NoteDownwardInterval(MIDINoteName first, MIDINoteName second)
        {
            return NoteUpwardInterval(second, first);
        }
        
        // handles negative numbers unlike % https://www.omnicalculator.com/math/modulo-of-negative-numbers
        private static int ModFloor(int a, int n)
        {
            return a - n * Mathf.FloorToInt((float)a/(float)n);
        }
    }
}