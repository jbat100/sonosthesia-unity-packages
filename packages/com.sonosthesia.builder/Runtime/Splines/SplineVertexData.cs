using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Builder
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SplineVertexData : SplineMesh.ISplineVertexData
    {
        public Vector3 position { get; set; }
        public Vector3 normal { get; set; }
        public Vector2 texture { get; set; }
    }
    
    public struct SplineVertexData4
    {
        public SplineVertexData v0, v1, v2, v3;
    }
}