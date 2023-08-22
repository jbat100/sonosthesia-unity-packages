using System.Runtime.CompilerServices;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Sonosthesia.Builder
{
    public static partial class Noise 
    {
        public struct VoronoiData 
        {
            public Sample4 a, b;
        }

        public interface IVoronoiFunction 
        {
            Sample4 Evaluate (VoronoiData minima);
        }
        
        public struct F1 : IVoronoiFunction 
        {

            public Sample4 Evaluate (VoronoiData data) => data.a;
        }

        public struct F2 : IVoronoiFunction 
        {
            public Sample4 Evaluate (VoronoiData data) => data.b;
        }

        public struct F2MinusF1 : IVoronoiFunction 
        {
            public Sample4 Evaluate (VoronoiData data) => data.b - data.a;
        }
        
        public interface IVoronoiDistance 
        {
            VoronoiData UpdateVoronoiData (VoronoiData data, Sample4 sample);

            VoronoiData InitialData { get; }
            
            Sample4 GetDistance (float4 x);

            Sample4 GetDistance (float4 x, float4 y);

            Sample4 GetDistance (float4 x, float4 y, float4 z);

            VoronoiData Finalize1D (VoronoiData minima);

            VoronoiData Finalize2D (VoronoiData minima);

            VoronoiData Finalize3D (VoronoiData minima);
        }
        
        public struct Worley : IVoronoiDistance 
        {
            public VoronoiData UpdateVoronoiData (VoronoiData data, Sample4 sample) 
            {
                bool4 newMinimum = sample.v < data.a.v;
                data.b = Sample4.Select(
                    Sample4.Select(data.b, sample, sample.v < data.b.v),
                    data.a,
                    newMinimum
                );
                data.a = Sample4.Select(data.a, sample, newMinimum);
                return data;
            }

            public VoronoiData InitialData => new VoronoiData 
            {
                a = new Sample4 { v = 2f },
                b = new Sample4 { v = 2f }
            };
            
            public Sample4 GetDistance (float4 x) => new Sample4 {
                v = abs(x),
                dx = select(-1f, 1f, x < 0f)
            };

            public Sample4 GetDistance (float4 x, float4 y) => GetDistance(x, 0f, y);

            public Sample4 GetDistance (float4 x, float4 y, float4 z) => new Sample4 {
                v = x * x + y * y + z * z,
                dx = x,
                dy = y,
                dz = z
            };

            public VoronoiData Finalize1D (VoronoiData minima) => minima;

            public VoronoiData Finalize2D (VoronoiData data) => Finalize3D(data);

            public VoronoiData Finalize3D (VoronoiData data)
            {
                bool4 keepA = data.a.v < 1f;
                data.a.v = select(1f, sqrt(data.a.v), keepA);
                data.a.dx = select(0f, -data.a.dx / data.a.v, keepA);
                data.a.dy = select(0f, -data.a.dy / data.a.v, keepA);
                data.a.dz = select(0f, -data.a.dz / data.a.v, keepA);

                bool4 keepB = data.b.v < 1f;
                data.b.v = select(1f, sqrt(data.b.v), keepB);
                data.b.dx = select(0f, -data.b.dx / data.b.v, keepB);
                data.b.dy = select(0f, -data.b.dy / data.b.v, keepB);
                data.b.dz = select(0f, -data.b.dz / data.b.v, keepB);
                return data;
            }
        }
        
        public struct SmoothWorley : IVoronoiDistance 
        {
            private const float smoothLSE = 10f, smoothPoly = 0.25f;
            
            public Sample4 GetDistance (float4 x) => default(Worley).GetDistance(x);

            public Sample4 GetDistance (float4 x, float4 y) => GetDistance(x, 0f, y);

            public Sample4 GetDistance (float4 x, float4 y, float4 z)
            {
                float4 v = sqrt(x * x + y * y + z * z);
                return new Sample4 {
                    v = v,
                    dx = x / -v,
                    dy = y / -v,
                    dz = y / -v
                };
            }

            public VoronoiData Finalize1D (VoronoiData data) 
            {
                data.a.dx /= data.a.v;
                data.a.v = log(data.a.v) / -smoothLSE;
                data.a = Sample4.Select(default, data.a.Smoothstep, data.a.v > 0f);
                data.b = Sample4.Select(default, data.b.Smoothstep, data.b.v > 0f);
                return data;
            }

            public VoronoiData Finalize2D (VoronoiData data) => Finalize3D(data);

            public VoronoiData Finalize3D(VoronoiData data)
            {
                data.a.dx /= data.a.v;
                data.a.dy /= data.a.v;
                data.a.dz /= data.a.v;
                data.a.v = log(data.a.v) / -smoothLSE;
                data.a = Sample4.Select(default, data.a.Smoothstep, data.a.v > 0f & data.a.v < 1f);
                data.b = Sample4.Select(default, data.b.Smoothstep, data.b.v > 0f & data.b.v < 1f);
                return data;
            }

            public VoronoiData UpdateVoronoiData (VoronoiData data, Sample4 sample) 
            {
                float4 e = exp(-smoothLSE * sample.v);
                data.a.v += e;
                data.a.dx += e * sample.dx;
                data.a.dy += e * sample.dy;
                data.a.dz += e * sample.dz;
                
                float4 h = 1f - abs(data.b.v - sample.v) / smoothPoly;
                
                float4
                    hdx = data.b.dx - sample.dx,
                    hdy = data.b.dy - sample.dy,
                    hdz = data.b.dz - sample.dz;
                bool4 ds = data.b.v - sample.v < 0f;
                hdx = select(-hdx, hdx, ds) * 0.5f * h;
                hdy = select(-hdy, hdy, ds) * 0.5f * h;
                hdz = select(-hdz, hdz, ds) * 0.5f * h;
                
                bool4 smooth = h > 0f;
                h = 0.25f * smoothPoly * h * h;
                data.b = Sample4.Select(data.b, sample, sample.v < data.b.v);
                data.b.v -= select(0f, h, smooth);
                data.b.dx -= select(0f, hdx, smooth);
                data.b.dy -= select(0f, hdy, smooth);
                data.b.dz -= select(0f, hdz, smooth);
                
                return data;
            }

            public VoronoiData InitialData => new VoronoiData {
                b = new Sample4 { v = 2f }
            };
        }
        
        public struct Chebyshev : IVoronoiDistance 
        {
            public VoronoiData UpdateVoronoiData (VoronoiData data, Sample4 sample) =>
                default(Worley).UpdateVoronoiData(data, sample);

            public VoronoiData InitialData => default(Worley).InitialData;
            
            public Sample4 GetDistance (float4 x) => default(Worley).GetDistance(x);

            public Sample4 GetDistance (float4 x, float4 y) {
                bool4 keepX = abs(x) > abs(y);
                return new Sample4 {
                    v = select(abs(y), abs(x), keepX),
                    dx = select(0f, select(-1f, 1f, x < 0f), keepX),
                    dz = select(select(-1f, 1f, y < 0f), 0f, keepX)
                };
            }

            public Sample4 GetDistance (float4 x, float4 y, float4 z) {
                bool4 keepX = abs(x) > abs(y) & abs(x) > abs(z);
                bool4 keepY = abs(y) > abs(z);
                return new Sample4 {
                    v = select(select(abs(z), abs(y), keepY), abs(x), keepX),
                    dx = select(0f, select(-1f, 1f, x < 0f), keepX),
                    dy = select(select(0f, select(-1f, 1f, y < 0f), keepY), 0f, keepX),
                    dz = select(select(select(-1f, 1f, z < 0f), 0f, keepY), 0f, keepX)
                };
            }

            public VoronoiData Finalize1D (VoronoiData minima) => minima;

            public VoronoiData Finalize2D (VoronoiData minima) => minima;

            public VoronoiData Finalize3D (VoronoiData minima) => minima;
        }
        
        public struct Voronoi1D<L, D, F> : INoise 
            where L : struct, ILattice 
            where D : struct, IVoronoiDistance
            where F : struct, IVoronoiFunction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                L l = default;
                D d = default;
                LatticeSpan4 x = l.GetLatticeSpan4(positions.c0, frequency);
                VoronoiData data = d.InitialData;
                for (int u = -1; u <= 1; u++) 
                {
                    SmallXXHash4 h = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
                    data = d.UpdateVoronoiData(data, d.GetDistance(h.Floats01A + u - x.g0));
                }
                Sample4 s = default(F).Evaluate(d.Finalize1D(data));
                s.dx *= frequency;
                return s;
            }
        }

        public struct Voronoi2D<L, D, F> : INoise 
            where L : struct, ILattice 
            where D : struct, IVoronoiDistance
            where F : struct, IVoronoiFunction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                L l = default;
                D d = default;
                LatticeSpan4
                    x = l.GetLatticeSpan4(positions.c0, frequency),
                    z = l.GetLatticeSpan4(positions.c2, frequency);

                VoronoiData data = d.InitialData;
                for (int u = -1; u <= 1; u++) 
                {
                    SmallXXHash4 hx = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
                    float4 xOffset = u - x.g0;
                    for (int v = -1; v <= 1; v++) 
                    {
                        SmallXXHash4 h = hx.Eat(l.ValidateSingleStep(z.p0 + v, frequency));
                        float4 zOffset = v - z.g0;
                        
                        data = d.UpdateVoronoiData(data, d.GetDistance(h.Floats01A + xOffset, h.Floats01B + zOffset));
                        data = d.UpdateVoronoiData(data, d.GetDistance(h.Floats01C + xOffset, h.Floats01D + zOffset));
                    }
                }
                Sample4 s = default(F).Evaluate(d.Finalize1D(data));
                s.dx *= frequency;
                return s;
            }
        }

        public struct Voronoi3D<L, D, F> : INoise 
            where L : struct, ILattice 
            where D : struct, IVoronoiDistance
            where F : struct, IVoronoiFunction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Sample4 GetNoise4 (float4x3 positions, SmallXXHash4 hash, int frequency) 
            {
                L l = default;
                D d = default;
                LatticeSpan4
                    x = l.GetLatticeSpan4(positions.c0, frequency),
                    y = l.GetLatticeSpan4(positions.c1, frequency),
                    z = l.GetLatticeSpan4(positions.c2, frequency);

                VoronoiData data = d.InitialData;
                for (int u = -1; u <= 1; u++) 
                {
                    SmallXXHash4 hx = hash.Eat(l.ValidateSingleStep(x.p0 + u, frequency));
                    float4 xOffset = u - x.g0;
                    for (int v = -1; v <= 1; v++) 
                    {
                        SmallXXHash4 hy = hx.Eat(l.ValidateSingleStep(y.p0 + v, frequency));
                        float4 yOffset = v - y.g0;
                        for (int w = -1; w <= 1; w++) 
                        {
                            SmallXXHash4 h = hy.Eat(l.ValidateSingleStep(z.p0 + w, frequency));
                            float4 zOffset = w - z.g0;
                            data = d.UpdateVoronoiData(data, d.GetDistance(
                                h.GetBitsAsFloats01(5, 0) + xOffset,
                                h.GetBitsAsFloats01(5, 5) + yOffset,
                                h.GetBitsAsFloats01(5, 10) + zOffset
                            ));
                            data = d.UpdateVoronoiData(data, d.GetDistance(
                                h.GetBitsAsFloats01(5, 15) + xOffset,
                                h.GetBitsAsFloats01(5, 20) + yOffset,
                                h.GetBitsAsFloats01(5, 25) + zOffset
                            ));
                        }
                    }
                }
                Sample4 s = default(F).Evaluate(d.Finalize1D(data));
                s.dx *= frequency;
                return s;
            }
        }
    }
}