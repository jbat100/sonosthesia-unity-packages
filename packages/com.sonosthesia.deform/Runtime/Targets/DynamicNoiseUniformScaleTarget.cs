using Sonosthesia.Noise;
using Sonosthesia.Target;

namespace Sonosthesia.Deform
{
    public class DynamicNoiseUniformScaleTarget : DynamicNoiseTarget<float, FloatBlender>
    {
        protected override float Reference => DynamicNoiseSettings.Domain.scale.x;

        protected override void ApplyBlended(float value)
        {
            SpaceTRS domain = DynamicNoiseSettings.Domain;
            domain.scale = value;
            DynamicNoiseSettings.Domain = domain;
        }
    }
}