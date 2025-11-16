namespace Sonosthesia.Trigger
{
    public enum AccumulationMode
    {
        None,
        Sum,
        Max,
        Min,
        Weighted
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