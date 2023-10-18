using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [ExecuteInEditMode]
    public class RegularTransformedGroupInstantiator : TransformedGroupInstantiator<GroupTransformerElement>
    {
        [SerializeField] private float _start;

        [SerializeField] private float _end = 1f;

        [SerializeField] private int _count = 10;

        [SerializeField] private bool _closed;

        protected override void OnValidate()
        {
            RequiredCount = _count;
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            RequiredCount = _count;
            base.OnEnable();
        }

        protected override IReadOnlyList<float> Offsets
        {
            get
            {
                List<float> result = new();
                float range = _end - _start;
                float increment = range / (_closed ? _count : _count - 1);
                float current = _start;
                for (int i = 0; i < _count; i++)
                {
                    result.Add(current);
                    current += increment;
                }
                return result.AsReadOnly();
            }
        }
    }
}