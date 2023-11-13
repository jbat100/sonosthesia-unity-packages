using Sonosthesia.Noise;
using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public class DynamicMeshNoiseUniformScaleTarget : DynamicMeshNoiseTarget<float, FloatBlender>
    {
        protected override float Reference => Domain.scale.x;

        protected override void ApplyBlended(float value)
        {
            SpaceTRS domain = Domain;
            domain.scale = value;
            Domain = domain;
        }
    }
}