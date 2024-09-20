namespace Sonosthesia.Utils
{
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
        
        public Peak(float magnitude, float duration, float strength)
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