namespace Sonosthesia.Generator
{
    public class TimeFloatGenerator : Generator<float>
    {
        public override float Evaluate(float time)
        {
            return time;
        }
    }
}