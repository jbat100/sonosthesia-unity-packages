namespace Sonosthesia.Utils
{
    // note : general Peak concept, can be reused in many contexts involving signals

    public readonly struct Peak
    {
        public readonly float Magnitude;
        public readonly float Duration;
        public readonly float Strength;

        public Peak(float magnitude, float duration)
        {
            Magnitude = magnitude;
            Duration = duration;
            Strength = 0;
        }
        
        public Peak(float duration, float magnitude, float strength)
        {
            Magnitude = magnitude;
            Duration = duration;
            Strength = strength;
        }

        public override string ToString()
        {
            return $"AudioPeak (Magnitude : {Magnitude}, Duration : {Duration})";
        }
    }
}