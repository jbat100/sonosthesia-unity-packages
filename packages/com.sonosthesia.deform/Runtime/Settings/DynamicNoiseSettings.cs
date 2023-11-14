using System;
using Sonosthesia.Noise;
using UnityEngine;

namespace Sonosthesia.Deform
{
    [Serializable]
    public class DynamicNoiseSettings
    {
        [Tooltip("Frequency of the noise component")] public int Frequency;
        [Tooltip("Magnitude of the displacement")] public float Displacement;
        [Tooltip("Velocity of dynamic change")] public float Velocity;
        [Tooltip("Expected range [0,2] or use Ping Pong")] public AnimationCurve LerpCurve;
        [Tooltip("Noise domain")] public SpaceTRS Domain = new () { scale = 1f };
    }
}