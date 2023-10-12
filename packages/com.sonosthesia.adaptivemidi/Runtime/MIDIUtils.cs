namespace Sonosthesia.AdaptiveMIDI
{
    public static class MIDIUtils
    {
        public static int To14BitInt(byte msb, byte lsb, bool signed)
        {
            // Ensure the values are 7-bit
            msb &= 0x7F;
            lsb &= 0x7F;

            // Combine MSB and LSB to get a 14-bit integer [0 16383]
            int combinedValue = (msb << 7) | lsb;

            return signed ? combinedValue - 8192 : combinedValue;
        }
    }
}