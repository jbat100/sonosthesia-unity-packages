namespace Sonosthesia.Trigger
{
    public enum AccumulationMode
    {
        None,
        Sum,
        Max,
        Min
    }

    public static class AccumulationModeExtensions
    {
        public static float Seed(this AccumulationMode mode)
        {
            return mode switch
            {
                AccumulationMode.Max => float.NegativeInfinity,
                AccumulationMode.Min => float.PositiveInfinity,
                _ => 0f
            };
        }
    }
}