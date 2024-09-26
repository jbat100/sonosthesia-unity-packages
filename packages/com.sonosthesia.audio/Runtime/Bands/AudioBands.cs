namespace Sonosthesia.Audio
{
    public readonly struct TriAudioBands
    {
        public readonly float B1;
        public readonly float B2;
        public readonly float B3;

        public TriAudioBands(float b1, float b2, float b3)
        {
            B1 = b1;
            B2 = b2;
            B3 = b3;
        }
    }
    
    public readonly struct QuintAudioBands
    {
        public readonly float B1;
        public readonly float B2;
        public readonly float B3;
        public readonly float B4;
        public readonly float B5;

        public QuintAudioBands(float b1, float b2, float b3, float b4, float b5)
        {
            B1 = b1;
            B2 = b2;
            B3 = b3;
            B4 = b4;
            B5 = b5;
        }
    }
}