namespace Sonosthesia.Mapping
{
    public class BoundFloatProvider : BoundProvider<float>
    {
        protected override float Randomize(float lower, float upper)
        {
            return UnityEngine.Random.Range(lower, upper);
        }
    }
}