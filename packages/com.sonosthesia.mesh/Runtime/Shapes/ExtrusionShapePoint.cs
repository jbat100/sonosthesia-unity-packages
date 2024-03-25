using System;
using UnityEngine;

namespace Sonosthesia.Mesh
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ExtrusionShapePoint
    {
        public Vector2 position { get; set; }
        public Vector2 normal { get; set; }
        public float u { get; set; }
    }
}