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
            public float4 t, dt;
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
                float4 t = coordinates - points;
                span.t = t * t * t * (t * (t * 6f - 15f) + 10f);
                span.dt = t * t * (t * (t * 30f - 60f) + 30f);
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

                span.dt = default;
                
                return span;
            }
            
            public int4 ValidateSingleStep (int4 points, int frequency)
            {
                return select(select(points, 0, points == frequency), frequency - 1, points == -1);
            }
        }

        public struct Lattice1D<G, L> : INoise, ISimpleNoise where G : IGradient, ISimpleGradient where L : ILattice
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4(float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                LatticeSpan4 x = default(L).GetLatticeSpan4(positions.c0, frequency);
                G g = default;
                Sample4 a = g.Evaluate(hash.Eat(x.p0), x.g0), b = g.Evaluate(hash.Eat(x.p1), x.g1);
                return g.EvaluateCombined(new Sample4 {
                    v = lerp(a.v, b.v, x.t),
                    dx = frequency * (lerp(a.dx, b.dx, x.t) + (b.v - a.v) * x.dt)
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float4 GetNoiseValue4(float4x3 positions, SmallXXHash4 hash, int frequency)
            {
                LatticeSpan4 x = default(L).GetLatticeSpan4(positions.c0, frequency);
                G g = default;
                float4 a = g.EvaluateValue(hash.Eat(x.p0), x.g0), b = g.EvaluateValue(hash.Eat(x.p1), x.g1);
                return g.EvaluateCombinedValue(lerp(a, b, x.t));
            }
        }
        
        public struct Lattice2D<G, L> : INoise, ISimpleNoise where G : IGradient, ISimpleGradient where L : ILattice
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4(float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                LatticeSpan4 
                    x = default(L).GetLatticeSpan4(positions.c0, frequency), 
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);
                SmallXXHash4 h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1);
                G g = default;
                Sample4
                    a = g.Evaluate(h0.Eat(z.p0), x.g0, z.g0),
                    b = g.Evaluate(h0.Eat(z.p1), x.g0, z.g1),
                    c = g.Evaluate(h1.Eat(z.p0), x.g1, z.g0),
                    d = g.Evaluate(h1.Eat(z.p1), x.g1, z.g1);

                return g.EvaluateCombined(new Sample4 {
                    v = lerp(lerp(a.v, b.v, z.t), lerp(c.v, d.v, z.t), x.t),
                    dx = frequency * (
                        lerp(lerp(a.dx, b.dx, z.t), lerp(c.dx, d.dx, z.t), x.t) +
                        (lerp(c.v, d.v, z.t) - lerp(a.v, b.v, z.t)) * x.dt
                    ),
                    dz = frequency * lerp(
                        lerp(a.dz, b.dz, z.t) + (b.v - a.v) * z.dt,
                        lerp(c.dz, d.dz, z.t) + (d.v - c.v) * z.dt,
                        x.t
                    )
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float4 GetNoiseValue4(float4x3 positions, SmallXXHash4 hash, int frequency)
            {
                LatticeSpan4 
                    x = default(L).GetLatticeSpan4(positions.c0, frequency), 
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);
                SmallXXHash4 h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1);
                G g = default;
                float4
                    a = g.EvaluateValue(h0.Eat(z.p0), x.g0, z.g0),
                    b = g.EvaluateValue(h0.Eat(z.p1), x.g0, z.g1),
                    c = g.EvaluateValue(h1.Eat(z.p0), x.g1, z.g0),
                    d = g.EvaluateValue(h1.Eat(z.p1), x.g1, z.g1);
                return g.EvaluateCombinedValue(lerp(lerp(a, b, z.t), lerp(c, d, z.t), x.t));
            }
        }
        
        public struct Lattice3D<G, L> : INoise, ISimpleNoise where G : struct, IGradient, ISimpleGradient where L : ILattice 
        {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) {
                LatticeSpan4
                    x = default(L).GetLatticeSpan4(positions.c0, frequency),
                    y = default(L).GetLatticeSpan4(positions.c1, frequency),
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);

                SmallXXHash4
                    h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1),
                    h00 = h0.Eat(y.p0), h01 = h0.Eat(y.p1),
                    h10 = h1.Eat(y.p0), h11 = h1.Eat(y.p1);

                G gradient = default;
                Sample4
                    a = gradient.Evaluate(h00.Eat(z.p0), x.g0, y.g0, z.g0),
                    b = gradient.Evaluate(h00.Eat(z.p1), x.g0, y.g0, z.g1),
                    c = gradient.Evaluate(h01.Eat(z.p0), x.g0, y.g1, z.g0),
                    d = gradient.Evaluate(h01.Eat(z.p1), x.g0, y.g1, z.g1),
                    e = gradient.Evaluate(h10.Eat(z.p0), x.g1, y.g0, z.g0),
                    f = gradient.Evaluate(h10.Eat(z.p1), x.g1, y.g0, z.g1),
                    g = gradient.Evaluate(h11.Eat(z.p0), x.g1, y.g1, z.g0),
                    h = gradient.Evaluate(h11.Eat(z.p1), x.g1, y.g1, z.g1);

                return gradient.EvaluateCombined(new Sample4 {
                    v = lerp(
                        lerp(lerp(a.v, b.v, z.t), lerp(c.v, d.v, z.t), y.t),
                        lerp(lerp(e.v, f.v, z.t), lerp(g.v, h.v, z.t), y.t),
                        x.t
                    ),
                    dx = frequency * (
                        lerp(
                            lerp(lerp(a.dx, b.dx, z.t), lerp(c.dx, d.dx, z.t), y.t),
                            lerp(lerp(e.dx, f.dx, z.t), lerp(g.dx, h.dx, z.t), y.t),
                            x.t
                        ) + (
                            lerp(lerp(e.v, f.v, z.t), lerp(g.v, h.v, z.t), y.t) -
                            lerp(lerp(a.v, b.v, z.t), lerp(c.v, d.v, z.t), y.t)
                        ) * x.dt
                    ),
                    dy = frequency * lerp(
                        lerp(lerp(a.dy, b.dy, z.t), lerp(c.dy, d.dy, z.t), y.t) +
                        (lerp(c.v, d.v, z.t) - lerp(a.v, b.v, z.t)) * y.dt,
                        lerp(lerp(e.dy, f.dy, z.t), lerp(g.dy, h.dy, z.t), y.t) +
                        (lerp(g.v, h.v, z.t) - lerp(e.v, f.v, z.t)) * y.dt,
                        x.t
                    ),
                    dz = frequency * lerp(
                        lerp(
                            lerp(a.dz, b.dz, z.t) + (b.v - a.v) * z.dt,
                            lerp(c.dz, d.dz, z.t) + (d.v - c.v) * z.dt,
                            y.t
                        ),
                        lerp(
                            lerp(e.dz, f.dz, z.t) + (f.v - e.v) * z.dt,
                            lerp(g.dz, h.dz, z.t) + (h.v - g.v) * z.dt,
                            y.t
                        ),
                        x.t
                    )
                });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float4 GetNoiseValue4(float4x3 positions, SmallXXHash4 hash, int frequency)
            {
                LatticeSpan4
                    x = default(L).GetLatticeSpan4(positions.c0, frequency),
                    y = default(L).GetLatticeSpan4(positions.c1, frequency),
                    z = default(L).GetLatticeSpan4(positions.c2, frequency);

                SmallXXHash4
                    h0 = hash.Eat(x.p0), h1 = hash.Eat(x.p1),
                    h00 = h0.Eat(y.p0), h01 = h0.Eat(y.p1),
                    h10 = h1.Eat(y.p0), h11 = h1.Eat(y.p1);

                G gradient = default;
                float4
                    a = gradient.EvaluateValue(h00.Eat(z.p0), x.g0, y.g0, z.g0),
                    b = gradient.EvaluateValue(h00.Eat(z.p1), x.g0, y.g0, z.g1),
                    c = gradient.EvaluateValue(h01.Eat(z.p0), x.g0, y.g1, z.g0),
                    d = gradient.EvaluateValue(h01.Eat(z.p1), x.g0, y.g1, z.g1),
                    e = gradient.EvaluateValue(h10.Eat(z.p0), x.g1, y.g0, z.g0),
                    f = gradient.EvaluateValue(h10.Eat(z.p1), x.g1, y.g0, z.g1),
                    g = gradient.EvaluateValue(h11.Eat(z.p0), x.g1, y.g1, z.g0),
                    h = gradient.EvaluateValue(h11.Eat(z.p1), x.g1, y.g1, z.g1);
                return gradient.EvaluateCombinedValue(lerp(
                    lerp(lerp(a, b, z.t), lerp(c, d, z.t), y.t),
                    lerp(lerp(e, f, z.t), lerp(g, h, z.t), y.t),
                    x.t
                ));
            }
        }
    }
}