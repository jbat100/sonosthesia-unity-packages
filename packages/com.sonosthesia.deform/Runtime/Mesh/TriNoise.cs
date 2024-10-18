using System.Runtime.InteropServices;
using Sonosthesia.Ease;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Deform
{
    // TODO: Think about having TriNoise with each phase having a separate TRS controlled by brownian motion or anything
    
    public static class TriNoise
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct NoisePhase
        {
            public readonly int Seed;
            public readonly float Displacement;

            public NoisePhase(int seed, float displacement)
            {
                Seed = seed;
                Displacement = displacement;
            }

            public override string ToString()
            {
                return $"({Seed} : {Displacement})";
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct TriNoisePhase
        {
            public readonly NoisePhase C1;
            public readonly NoisePhase C2;
            public readonly NoisePhase C3;

            public TriNoisePhase(NoisePhase c1, NoisePhase c2, NoisePhase c3)
            {
                C1 = c1;
                C2 = c2;
                C3 = c3;
            }

            public override string ToString()
            {
                return $"({nameof(C1)} : {C1}, {nameof(C2)} : {C2}, {nameof(C3)} : {C3})";
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct TriNoiseComponent
        {
            public readonly int Frequency;
            public readonly TriNoisePhase TriPhase;

            public TriNoiseComponent(int frequency, TriNoisePhase triPhase)
            {
                Frequency = frequency;
                TriPhase = triPhase;
            }

            public override string ToString()
            {
                return $"({nameof(Frequency)} : {Frequency}, {nameof(TriPhase)} : {TriPhase})";
            }
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct DomainNoiseComponent
        {
            public readonly TriNoiseComponent Component;
            public readonly float3x4 DomainTRS;
            public readonly float3x3 DerivativeMatrix;

            public DomainNoiseComponent(TriNoiseComponent component, float3x4 domainTRS, float3x3 derivativeMatrix)
            {
                Component = component;
                DomainTRS = domainTRS;
                DerivativeMatrix = derivativeMatrix;
            }
        }

        private const float ONE_THIRD = 1f / 3f;
        
        public static TriNoiseComponent GetNoiseComponent(DynamicNoiseSettings settings, int seed, float displacement, float localTime)
        {
            NoisePhase GetPhase(int index)
            {
                float time = localTime + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                return new NoisePhase(
                    seed + flooredTime + index, 
                    settings.LerpCurve.Evaluate((time - flooredTime) * 2) * displacement * settings.Displacement);
            }

            return new TriNoiseComponent(settings.Frequency, new TriNoisePhase(GetPhase(0), GetPhase(1), GetPhase(2)));
        }

        public static TriNoiseComponent GetNoiseComponent(CompoundMeshNoiseInfo info, int seed)
        {
            NoisePhase GetPhase(int index)
            {
                float time = info.time + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                float fractionalTime = (time - flooredTime) * 2f;
                float crossFade = fractionalTime < 1f
                    ? info.crossFadeType.Evaluate(0f, 1f, fractionalTime)
                    : info.crossFadeType.Evaluate(1f, 0f, fractionalTime - 1f);
                return new NoisePhase(
                    seed + flooredTime + index, 
                    crossFade * info.displacement);
            }

            return new TriNoiseComponent(info.frequency, new TriNoisePhase(GetPhase(0), GetPhase(1), GetPhase(2)));
        }
    }
}