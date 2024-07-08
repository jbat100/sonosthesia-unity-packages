namespace Sonosthesia.Signal
{
    public readonly struct TriBand<T> where T : struct
    {
        public readonly T Low;
        public readonly T Mid;
        public readonly T High;

        public TriBand(T low, T mid, T high)
        {
            Low = low;
            Mid = mid;
            High = high;
        }
    }
}