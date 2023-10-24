using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Spawn
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