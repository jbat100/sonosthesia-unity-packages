using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Builder
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SplineVertexData : SplineMesh.ISplineVertexData
    {
        public Vector3 position { get; set; }
        public Vector3 normal { get; set; }
        public Vector2 texture { get; set; }
    }
}