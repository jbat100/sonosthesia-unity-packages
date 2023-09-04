using Sonosthesia.Flow;

namespace Sonosthesia.Builder
{
    public class DynamicMeshNoiseUniformScaleTarget : DynamicMeshNoiseTarget<float>
    {
        private float _reference;

        protected override void Awake()
        {
            base.Awake();
            _reference = Domain.scale.x;
        }

        protected override void Apply(float value)
        {
            SpaceTRS domain = Domain;
            domain.scale = TargetBlend.Blend(_reference, value);
            Domain = domain;
        }
    }
}