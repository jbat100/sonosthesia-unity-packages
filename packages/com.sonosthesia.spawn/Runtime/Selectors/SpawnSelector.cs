using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class SpawnSelector : Selector<SpawnPayload>
    {
        private enum Selection
        {
            None,
            Unit
        }

        [SerializeField] private Selection _selection;

        protected override float InternalSelect(SpawnPayload value)
        {
            return _selection switch
            {
                Selection.None => 0f,
                Selection.Unit => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}