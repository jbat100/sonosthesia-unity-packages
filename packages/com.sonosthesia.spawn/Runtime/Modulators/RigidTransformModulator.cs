using System;
using Sonosthesia.Arpeggiator;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Spawn
{
    public abstract class RigidTransformModulator : Modulator<RigidTransform>
    {
        private enum Blend
        {
            Override,
            Combine
        }

        [SerializeField] private Blend _blend;

        public sealed override RigidTransform Modulate(RigidTransform original, float offset)
        {
            return _blend switch
            {
                Blend.Override => Modulation(offset),
                Blend.Combine => math.mul(Modulation(offset), original),
                _ => throw new NotImplementedException()
            };
        }

        protected abstract RigidTransform Modulation(float offset);
    }
}
