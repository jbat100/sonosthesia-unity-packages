using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public static partial class Noise {

        public struct Simplex1D<G> : INoise where G : struct, IGradient {

            public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
                return default(G).EvaluateAfterInterpolation(0f);
            }
        }

        public struct Simplex2D<G> : INoise where G : struct, IGradient {

            public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
                return default(G).EvaluateAfterInterpolation(0f);
            }
        }

        public struct Simplex3D<G> : INoise where G : struct, IGradient {

            public float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
                return default(G).EvaluateAfterInterpolation(0f);
            }
        }
    }
}