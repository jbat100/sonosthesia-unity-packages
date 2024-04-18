using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Mesh
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ExtrusionPoint
    {
        public Vector2 position { get; set; }
        public Vector2 normal { get; set; }
        public float u { get; set; }
    }

    public static class ExtrusionPointUtils
    {
        public static TVertex MakeVertex<TVertex>(this ExtrusionPoint point, float3 position, quaternion rotation, float scale)
            where TVertex: struct, SplineMesh.ISplineVertexData
        {
            throw new NotImplementedException();
        }
    }
}