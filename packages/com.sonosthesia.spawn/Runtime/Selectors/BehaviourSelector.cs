using System;
using Sonosthesia.Flow;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public class BehaviourSelector : Selector<BehaviourPayload>
    {
        public enum Selection
        {
            None,
            Excitation,
            Size,
            Hue,
            Opacity
        }

        [SerializeField] private Selection _selection;

        protected override float InternalSelect(BehaviourPayload value)
        {
            return _selection switch
            {
                Selection.None => 0f,
                Selection.Excitation => value.Excitation,
                Selection.Size => value.Size,
                Selection.Hue => value.Hue,
                Selection.Opacity => value.Opacity,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}