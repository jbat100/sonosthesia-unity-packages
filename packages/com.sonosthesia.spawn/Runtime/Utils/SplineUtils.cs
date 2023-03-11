using System;
using Unity.Mathematics;
using UnityEngine.Splines;

namespace Sonosthesia.Spawn
{
    public enum SplineVector
    {
        Position,
        Tangent,
        Up
    }
    
    public static class SplineUtils
    {
        public static quaternion Rotation(this Spline spline, float t, SplineVector forward, SplineVector up)
        {
            return quaternion.LookRotationSafe(spline.Vector(t, forward), spline.Vector(t, up)); 
        }
        
        public static float3 Vector(this Spline spline, float t, SplineVector component)
        {
            spline.Evaluate(t, out float3 position, out float3 tangent, out float3 up);
            return component switch
            {
                SplineVector.Position => position,
                SplineVector.Tangent => tangent,
                SplineVector.Up => up,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}