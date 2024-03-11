using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    public class LineGroupTransformerConfiguration : GroupTransformerConfiguration
    {
        [SerializeField] private Vector3 _start;

        [SerializeField] private Vector3 _end;

        public override void Apply<T>(IEnumerable<T> targets)
        {
            Vector3 difference = _end - _start;
            foreach (T element in targets)
            {
                element.Transform.localPosition = _start + difference * element.Offset;
            }
        }
    }
}