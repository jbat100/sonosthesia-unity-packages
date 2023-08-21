using System.Runtime.CompilerServices;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public static partial class Noise
    {
        public interface ILattice 
        {
            LatticeSpan4 GetLatticeSpan4 (float4 coordinates, int frequency);
            
            int4 ValidateSingleStep (int4 points, int frequency);
        }
        
        public struct LatticeSpan4 
        {
            public int4 p0, p1;
            public float4 g0, g1;
            public float4 t;
        }

        public struct LatticeNormal : ILattice
        {
            public LatticeSpan4 GetLatticeSpan4(float4 coordinates, int frequency)
            {
                coordinates *= frequency;
                float4 points = floor(coordinates);
                LatticeSpan4 span;
                span.p0 = (int4)points;
                span.p1 = span.p0 + 1;
                span.g0 = coordinates - span.p0;
                span.g1 = span.g0 - 1f;
                span.t = coordinates - points;
                span.t = span.t * span.t * span.t * (span.t * (span.t * 6f - 15f) + 10f);
                return span;
            }
            
            public int4 ValidateSingleStep (int4 points, int frequency) => points;
        }
        
        public struct LatticeTiling : ILattice 
        {
            public LatticeSpan4 GetLatticeSpan4 (float4 coordinates, int frequency) 
            {
                coordinates *= frequency;
                float4 points = floor(coordinates);
                LatticeSpan4 span;
                span.p0 = (int4)points;
                span.p1 = span.p0 + 1;
                span.g0 = coordinates - span.p0;
                span.g1 = span.g0 - 1f;

                span.p0 -= (int4)ceil(points / frequency) * frequency;
                span.p0 = select(span.p0, span.p0 + frequency, span.p0 < 0);
                span.p1 = span.p0 + 1;
                span.p1 = select(span.p1, 0, span.p1 == frequency);

                span.t = coordinates - points;
                span.t = span.t * span.t * span.t * (span.t * (span.t * 6f - 15f) + 10f);
                return span;
            }
            
            public int4 ValidateSingleStep (int4 points, int frequency)
            {
                return select(select(points, 0, points == frequency), frequency - 1, points == -1);
            }
        }

        public struct Lattice1D<G, L> : INoise where G : IGradient where L : ILattice
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4(float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                LatticeSpan4 x = default(L).GetLatticeSpan4(positions.c0, frequency);
                G g = default;
                float4 raw =  lerp(g.Evaluate(hash.Eat(x.p0), x.g0), g.Evaluate(hash.Eat(x.p1), x.g1), x.t) * 2f - 1f;
                return g.EvaluateCombined(raw);
            }
        }
        
        public struct Lattice2D<G, L> : INoise where G : IGradient where L : ILattice
        {
            public Sample4 GetNoise4(float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                LatticeSpan4 
                    x = default(L).GetLatticeSpan4(positions.c0, frequency), 
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);
                SmallXXHash4 h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1);
                G g = default;
                float4 raw = lerp(
                    lerp(
                        g.Evaluate(h0.Eat(z.p0), x.g0, z.g0),
                        g.Evaluate(h0.Eat(z.p1), x.g0, z.g1),
                        z.t
                    ),
                    lerp(
                        g.Evaluate(h1.Eat(z.p0), x.g1, z.g0),
                        g.Evaluate(h1.Eat(z.p1), x.g1, z.g1),
                        z.t
                    ),
                    x.t
                );
                return g.EvaluateCombined(raw);
            }
        }
        
        public struct Lattice3D<G, L> : INoise where G : struct, IGradient where L : ILattice {

            public Sample4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
                LatticeSpan4
                    x = default(L).GetLatticeSpan4(positions.c0, frequency),
                    y = default(L).GetLatticeSpan4(positions.c1, frequency),
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);

                SmallXXHash4
                    h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1),
                    h00 = h0.Eat(y.p0), h01 = h0.Eat(y.p1),
                    h10 = h1.Eat(y.p0), h11 = h1.Eat(y.p1);

                G g = default;
                
                float4 raw = lerp(
                    lerp(
                        lerp(
                            g.Evaluate(h00.Eat(z.p0), x.g0, y.g0, z.g0),
                            g.Evaluate(h00.Eat(z.p1), x.g0, y.g0, z.g1),
                            z.t
                        ),
                        lerp(
                            g.Evaluate(h01.Eat(z.p0), x.g0, y.g1, z.g0),
                            g.Evaluate(h01.Eat(z.p1), x.g0, y.g1, z.g1),
                            z.t
                        ),
                        y.t
                    ),
                    lerp(
                        lerp(
                            g.Evaluate(h10.Eat(z.p0), x.g1, y.g0, z.g0),
                            g.Evaluate(h10.Eat(z.p1), x.g1, y.g0, z.g1),
                            z.t
                        ),
                        lerp(
                            g.Evaluate(h11.Eat(z.p0), x.g1, y.g1, z.g0),
                            g.Evaluate(h11.Eat(z.p1), x.g1, y.g1, z.g1),
                            z.t
                        ),
                        y.t
                    ),
                    x.t
                );
                return g.EvaluateCombined(raw);
            }
        }
    }
}