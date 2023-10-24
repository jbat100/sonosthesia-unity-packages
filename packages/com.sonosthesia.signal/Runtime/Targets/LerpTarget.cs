using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Flow
{
    public class LerpTarget : Target<float>
    {
        [SerializeField] private Lerper _groupLerper;

        protected override void Awake()
        {
            base.Awake();
            if (!_groupLerper)
            {
                _groupLerper = GetComponent<Lerper>();
            }
        }

        protected override void Apply(float value) => _groupLerper.Lerp = value;
    }
}