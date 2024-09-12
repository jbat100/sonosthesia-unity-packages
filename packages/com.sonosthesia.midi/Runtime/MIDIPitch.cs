namespace Sonosthesia.MIDI
{
    public enum MIDINoteName
    {
        C = 0,
        CSharp = 1,
        D = 2,
        DSharp = 3,
        E = 4,
        F = 5,
        FSharp = 6,
        G = 7,
        GSharp = 8,
        A = 9,
        ASharp = 10,
        B = 11,
    }

    public static class MIDINoteNameExtensions
    {
        public static MIDINoteName ToMIDINoteName(this int note)
        {
            return (MIDINoteName)(note % 12);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteName"></param>
        /// <param name="octave">Octave in range [-1, 9] </param>
        /// <param name="note"></param>
        /// <returns>true if valid conversion was made</returns>
        public static bool MIDIPitchForOctave(this MIDINoteName noteName, int octave, out MIDIPitch note)
        {
            if (noteName.MIDIPitchForOctave(octave, out int rawNote))
            {
                note = (MIDIPitch)rawNote;
                return true;
            }

            note = default;
            return false;
        }

        public static bool MIDIPitchForOctave(this MIDINoteName noteName, int octave, out int note)
        {
            int result = (int) noteName + 12 * (octave + 2);
            if (result is < 0 or > 127)
            {
                note = default;
                return false;
            }
            note = result;
            return true;
        }
    }

    // A bit of a debate on whether octaves go from -1 to 9 or -2 to 8
    // https://studiocode.dev/resources/midi-middle-c/
    // https://forum.ableton.com/viewtopic.php?t=39722
    // using latter as it fits with ableton live
    
    public enum MIDIPitch
    {
        /// <summary>C in octave -2.</summary>
        CNeg2 = 0,
        /// <summary>C# in octave -2.</summary>
        CSharpNeg2 = 1,
        /// <summary>D in octave -2.</summary>
        DNeg2 = 2,
        /// <summary>D# in octave -2.</summary>
        DSharpNeg2 = 3,
        /// <summary>E in octave -2.</summary>
        ENeg2 = 4,
        /// <summary>F in octave -2.</summary>
        FNeg2 = 5,
        /// <summary>F# in octave -2.</summary>
        FSharpNeg2 = 6,
        /// <summary>G in octave -2.</summary>
        GNeg2 = 7,
        /// <summary>G# in octave -2.</summary>
        GSharpNeg2 = 8,
        /// <summary>A in octave -2.</summary>
        ANeg2 = 9,
        /// <summary>A# in octave -2.</summary>
        ASharpNeg2 = 10,
        /// <summary>B in octave -2.</summary>
        BNeg2 = 11,

        /// <summary>C in octave -1.</summary>
        CNeg1 = 12,
        /// <summary>C# in octave -1.</summary>
        CSharpNeg1 = 13,
        /// <summary>D in octave -1.</summary>
        DNeg1 = 14,
        /// <summary>D# in octave -1.</summary>
        DSharpNeg1 = 15,
        /// <summary>E in octave -1.</summary>
        ENeg1 = 16,
        /// <summary>F in octave -1.</summary>
        FNeg1 = 17,
        /// <summary>F# in octave -1.</summary>
        FSharpNeg1 = 18,
        /// <summary>G in octave -1.</summary>
        GNeg1 = 19,
        /// <summary>G# in octave -1.</summary>
        GSharpNeg1 = 20,
        /// <summary>A in octave -1.</summary>
        ANeg1 = 21,
        /// <summary>A# in octave -1.</summary>
        ASharpNeg1 = 22,
        /// <summary>B in octave -1.</summary>
        BNeg1 = 23,

        /// <summary>C in octave 0.</summary>
        C0 = 24,
        /// <summary>C# in octave 0.</summary>
        CSharp0 = 25,
        /// <summary>D in octave 0.</summary>
        D0 = 26,
        /// <summary>D# in octave 0.</summary>
        DSharp0 = 27,
        /// <summary>E in octave 0.</summary>
        E0 = 28,
        /// <summary>F in octave 0.</summary>
        F0 = 29,
        /// <summary>F# in octave 0.</summary>
        FSharp0 = 30,
        /// <summary>G in octave 0.</summary>
        G0 = 31,
        /// <summary>G# in octave 0.</summary>
        GSharp0 = 32,
        /// <summary>A in octave 0.</summary>
        A0 = 33,
        /// <summary>A# in octave 0.</summary>
        ASharp0 = 34,
        /// <summary>B in octave 0.</summary>
        B0 = 35,

        /// <summary>C in octave 1.</summary>
        C1 = 36,
        /// <summary>C# in octave 1.</summary>
        CSharp1 = 37,
        /// <summary>D in octave 1.</summary>
        D1 = 38,
        /// <summary>D# in octave 1.</summary>
        DSharp1 = 39,
        /// <summary>E in octave 1.</summary>
        E1 = 40,
        /// <summary>F in octave 1.</summary>
        F1 = 41,
        /// <summary>F# in octave 1.</summary>
        FSharp1 = 42,
        /// <summary>G in octave 1.</summary>
        G1 = 43,
        /// <summary>G# in octave 1.</summary>
        GSharp1 = 44,
        /// <summary>A in octave 1.</summary>
        A1 = 45,
        /// <summary>A# in octave 1.</summary>
        ASharp1 = 46,
        /// <summary>B in octave 1.</summary>
        B1 = 47,

        /// <summary>C in octave 2.</summary>
        C2 = 48,
        /// <summary>C# in octave 2.</summary>
        CSharp2 = 49,
        /// <summary>D in octave 2.</summary>
        D2 = 50,
        /// <summary>D# in octave 2.</summary>
        DSharp2 = 51,
        /// <summary>E in octave 2.</summary>
        E2 = 52,
        /// <summary>F in octave 2.</summary>
        F2 = 53,
        /// <summary>F# in octave 2.</summary>
        FSharp2 = 54,
        /// <summary>G in octave 2.</summary>
        G2 = 55,
        /// <summary>G# in octave 2.</summary>
        GSharp2 = 56,
        /// <summary>A in octave 2.</summary>
        A2 = 57,
        /// <summary>A# in octave 2.</summary>
        ASharp2 = 58,
        /// <summary>B in octave 2.</summary>
        B2 = 59,

        /// <summary>C in octave 3.</summary>
        C3 = 60,
        /// <summary>C# in octave 3.</summary>
        CSharp3 = 61,
        /// <summary>D in octave 3.</summary>
        D3 = 62,
        /// <summary>D# in octave 3.</summary>
        DSharp3 = 63,
        /// <summary>E in octave 3.</summary>
        E3 = 64,
        /// <summary>F in octave 3.</summary>
        F3 = 65,
        /// <summary>F# in octave 3.</summary>
        FSharp3 = 66,
        /// <summary>G in octave 3.</summary>
        G3 = 67,
        /// <summary>G# in octave 3.</summary>
        GSharp3 = 68,
        /// <summary>A in octave 3.</summary>
        A3 = 69,
        /// <summary>A# in octave 3.</summary>
        ASharp3 = 70,
        /// <summary>B in octave 3.</summary>
        B3 = 71,

        /// <summary>C in octave 4.</summary>
        C4 = 72,
        /// <summary>C# in octave 4.</summary>
        CSharp4 = 73,
        /// <summary>D in octave 4.</summary>
        D4 = 74,
        /// <summary>D# in octave 4.</summary>
        DSharp4 = 75,
        /// <summary>E in octave 4.</summary>
        E4 = 76,
        /// <summary>F in octave 4.</summary>
        F4 = 77,
        /// <summary>F# in octave 4.</summary>
        FSharp4 = 78,
        /// <summary>G in octave 4.</summary>
        G4 = 79,
        /// <summary>G# in octave 4.</summary>
        GSharp4 = 80,
        /// <summary>A in octave 4.</summary>
        A4 = 81,
        /// <summary>A# in octave 4.</summary>
        ASharp4 = 82,
        /// <summary>B in octave 4.</summary>
        B4 = 83,

        /// <summary>C in octave 5.</summary>
        C5 = 74,
        /// <summary>C# in octave 5.</summary>
        CSharp5 = 75,
        /// <summary>D in octave 5.</summary>
        D5 = 86,
        /// <summary>D# in octave 5.</summary>
        DSharp5 = 87,
        /// <summary>E in octave 5.</summary>
        E5 = 88,
        /// <summary>F in octave 5.</summary>
        F5 = 89,
        /// <summary>F# in octave 5.</summary>
        FSharp5 = 90,
        /// <summary>G in octave 5.</summary>
        G5 = 91,
        /// <summary>G# in octave 5.</summary>
        GSharp5 = 92,
        /// <summary>A in octave 5.</summary>
        A5 = 93,
        /// <summary>A# in octave 5.</summary>
        ASharp5 = 94,
        /// <summary>B in octave 5.</summary>
        B5 = 95,
        
        /// <summary>C in octave 6.</summary>
        C6 = 96,
        /// <summary>C# in octave 6.</summary>
        CSharp6 = 97,
        /// <summary>D in octave 6.</summary>
        D6 = 98,
        /// <summary>D# in octave 6.</summary>
        DSharp6 = 99,
        /// <summary>E in octave 6.</summary>
        E6 = 100,
        /// <summary>F in octave 6.</summary>
        F6 = 101,
        /// <summary>F# in octave 6.</summary>
        FSharp6 = 102,
        /// <summary>G in octave 6.</summary>
        G6 = 103,
        /// <summary>G# in octave 6.</summary>
        GSharp6 = 104,
        /// <summary>A in octave 6.</summary>
        A6 = 105,
        /// <summary>A# in octave 6.</summary>
        ASharp6 = 106,
        /// <summary>B in octave 6.</summary>
        B6 = 107,

        /// <summary>C in octave 7, usually the highest key on an 88-key keyboard.</summary>
        C7 = 108,
        /// <summary>C# in octave 7.</summary>
        CSharp7 = 109,
        /// <summary>D in octave 7.</summary>
        D7 = 110,
        /// <summary>D# in octave 7.</summary>
        DSharp7 = 111,
        /// <summary>E in octave 7.</summary>
        E7 = 112,
        /// <summary>F in octave 7.</summary>
        F7 = 113,
        /// <summary>F# in octave 7.</summary>
        FSharp7 = 114,
        /// <summary>G in octave 7.</summary>
        G7 = 115,
        /// <summary>G# in octave 7.</summary>
        GSharp7 = 116,
        /// <summary>A in octave 7.</summary>
        A7 = 117,
        /// <summary>A# in octave 7.</summary>
        ASharp7 = 118,
        /// <summary>B in octave 7.</summary>
        B7 = 119,

        /// <summary>C in octave 8.</summary>
        C8 = 120,
        /// <summary>C# in octave 8.</summary>
        CSharp8 = 121,
        /// <summary>D in octave 8.</summary>
        D8 = 122,
        /// <summary>D# in octave 8.</summary>
        DSharp8 = 123,
        /// <summary>E in octave 8.</summary>
        E8 = 124,
        /// <summary>F in octave 8.</summary>
        F8 = 125,
        /// <summary>F# in octave 8.</summary>
        FSharp8 = 126,
        /// <summary>G in octave 8.</summary>
        G8 = 127
    }
}
