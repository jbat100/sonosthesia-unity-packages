using Sonosthesia.Utils;
using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Target
{
    public class TransformTransTarget : Target<Trans>
    {
        [SerializeField] private bool _applyPosition = true;
        [SerializeField] private bool _applyRotation = true;
        [SerializeField] private bool _applyScale = true;
        
        protected override void Apply(Trans value)
        {
            value.ApplyTo(transform, _applyScale, _applyPosition, _applyRotation);
        }
    }
}