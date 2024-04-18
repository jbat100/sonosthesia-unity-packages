using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    public class CustomGroupTransformerTester : GroupTransformerTester
    {
        [SerializeField] private List<float> _offsets;

        protected override IEnumerable<float> Offsets => _offsets.AsReadOnly();
    }
}