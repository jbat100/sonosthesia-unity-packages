using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Sonosthesia.Builder
{
    public static class TriNoise
    {
        
        
        #region job data
        
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
        public readonly struct TriPhase
        {
            public readonly NoisePhase C1;
            public readonly NoisePhase C2;
            public readonly NoisePhase C3;

            public TriPhase(NoisePhase c1, NoisePhase c2, NoisePhase c3)
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
        public readonly struct NoiseComponent
        {
            public readonly int Frequency;
            public readonly TriPhase TriPhase;

            public NoiseComponent(int frequency, TriPhase triPhase)
            {
                Frequency = frequency;
                TriPhase = triPhase;
            }

            public override string ToString()
            {
                return $"({nameof(Frequency)} : {Frequency}, {nameof(TriPhase)} : {TriPhase})";
            }
        }
        
        #endregion job data
        
        private const float ONE_THIRD = 1f / 3f;
        
        public static NoiseComponent GetNoiseComponent(DynamicSettings settings, int seed, float displacement, float localTime)
        {
            NoisePhase GetPhase(int index)
            {
                float time = localTime + index * ONE_THIRD;
                int flooredTime = Mathf.FloorToInt(time);
                return new NoisePhase(
                    seed + flooredTime + index, 
                    settings.LerpCurve.Evaluate((time - flooredTime) * 2) * displacement * settings.Displacement);
            }

            return new NoiseComponent(settings.Frequency, new TriPhase(GetPhase(0), GetPhase(1), GetPhase(2)));
        }
    }
}