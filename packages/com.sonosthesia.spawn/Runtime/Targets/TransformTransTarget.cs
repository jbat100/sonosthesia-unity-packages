using Sonosthesia.Flow;
using Sonosthesia.Utils;

namespace Sonosthesia.Spawn
{
    public class TransformTransTarget : Target<Trans>
    {
        protected override void Apply(Trans value)
        {
            value.ApplyTo(transform);
        }
    }
}