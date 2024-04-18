using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    
    /// <summary>
    /// Interface for Spline mesh vertex data. Implement this interface if you are extruding custom mesh data and
    /// do not want to use the vertex layout provided by <see cref="SplineMesh"/>."/>.
    /// </summary>
    public interface IVertexData
    {
        /// <summary>
        /// Vertex position.
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// Vertex normal.
        /// </summary>
        public Vector3 normal { get; set; }

        /// <summary>
        /// Vertex texture, corresponds to UV0.
        /// </summary>
        public Vector2 texture { get; set; }
    }
    
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexData : IVertexData
    {
        public Vector3 position { get; set; }
        public Vector3 normal { get; set; } 
        public Vector2 texture { get; set; }
    }

    public enum ExtrusionVStrategy
    {
        None,
        Unit,
        Length,
        NormalizedLength,
        Range,
        NormalizedRange
    }
    
    public readonly struct ExtrusionSettings
    {
        private const float k_ScaleMin = .00001f, k_ScaleMax = 10000f;
        private const int k_SegmentsMin = 2, k_SegmentsMax = 4096;

        public readonly float length;
        public readonly int segments;
        public readonly bool closed;
        public readonly float2 range;
        public readonly float scale;
        public readonly float fade;
        public readonly ExtrusionVStrategy vStrategy;

        public ExtrusionSettings(float length, int segments, bool closed, float2 range, float scale, float fade, ExtrusionVStrategy vStrategy)
        {
            this.length = length;
            this.segments = math.clamp(segments, k_SegmentsMin, k_SegmentsMax);
            this.range = new float2(math.min(range.x, range.y), math.max(range.x, range.y));
            this.closed = math.abs(1f - (this.range.y - this.range.x)) < float.Epsilon && closed;
            this.scale = math.clamp(scale, k_ScaleMin, k_ScaleMax);
            this.fade = fade;
            this.vStrategy = vStrategy;
        }
    }
    
    internal readonly struct ExtrudeInfo
    {
        public readonly float t;
        public readonly float v;
        public readonly float length;
        public readonly float scale;
        public readonly int start;
        public readonly bool closed;

        public ExtrudeInfo(float t, float v, float length, float scale, int start, bool closed)
        {
            this.t = t;
            this.v = v;
            this.length = length;
            this.scale = scale;
            this.start = start;
            this.closed = closed;
        }

        public ExtrudeInfo(ExtrusionSettings extrusionSettings, int index, int start)
        {
            float s = index / (extrusionSettings.segments - 1f);
            float t = math.lerp(extrusionSettings.range.x, extrusionSettings.range.y, s);
            float fade = extrusionSettings.fade == 0f ? 1f : math.smoothstep(0f, extrusionSettings.fade, math.abs(math.round(s) - s));
            float scale = fade * extrusionSettings.scale;
            float v = Extrusion.ComputeV(extrusionSettings.vStrategy, s, t, extrusionSettings.length, extrusionSettings.range);
            
            this.t = t;
            this.v = v;
            this.scale = scale;
            this.start = start;
            this.length = extrusionSettings.length;
            this.closed = extrusionSettings.closed;
        }

        public static ExtrudeInfo StartCap(ExtrusionSettings extrusionSettings, int start)
        {
            float2 range = extrusionSettings.closed ? math.frac(extrusionSettings.range) : math.clamp(extrusionSettings.range, 0f, 1f);
            float s = 0f;
            float t = math.lerp(range.x, range.y, s);
            float fade = extrusionSettings.fade == 0f ? 1f : math.smoothstep(0f, extrusionSettings.fade, math.abs(math.round(s) - s));
            float scale = fade * extrusionSettings.scale;
            float v = Extrusion.ComputeV(extrusionSettings.vStrategy, s, t, extrusionSettings.length, extrusionSettings.range);
            return new ExtrudeInfo(t, v, extrusionSettings.length, scale, start, extrusionSettings.closed);
        }
        
        public static ExtrudeInfo EndCap(ExtrusionSettings extrusionSettings, int start)
        {
            float2 range = extrusionSettings.closed ? math.frac(extrusionSettings.range) : math.clamp(extrusionSettings.range, 0f, 1f);
            float s = 1f;
            float t = math.lerp(range.x, range.y, s);
            float fade = extrusionSettings.fade == 0f ? 1f : math.smoothstep(0f, extrusionSettings.fade, math.abs(math.round(s) - s));
            float scale = fade * extrusionSettings.scale;
            float v = Extrusion.ComputeV(extrusionSettings.vStrategy, s, t, extrusionSettings.length, extrusionSettings.range);
            return new ExtrudeInfo(t, v, extrusionSettings.length, scale, start, extrusionSettings.closed);
        }
    }
    
    public static class Extrusion
    {
        public static readonly VertexAttributeDescriptor[] PipeVertexAttribs = new VertexAttributeDescriptor[]
        {
            new(VertexAttribute.Position),
            new(VertexAttribute.Normal),
            new(VertexAttribute.TexCoord0, dimension: 2)
        };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">Normalized along range</param>
        /// <param name="t">Normalized along spline</param>
        /// <param name="length">Spline length</param>
        /// <param name="range">Extrusion Range</param>
        /// <returns></returns>
        public static float ComputeV(ExtrusionVStrategy vStrategy, float s, float t, float length, float2 range)
        {
            return vStrategy switch
            {
                ExtrusionVStrategy.Unit => 1f,
                ExtrusionVStrategy.Length => t * length,
                ExtrusionVStrategy.NormalizedLength => t,
                ExtrusionVStrategy.Range => s * (range.y - range.x),
                ExtrusionVStrategy.NormalizedRange => s,
                _ => 0f
            };
        }
        
        
    }
}