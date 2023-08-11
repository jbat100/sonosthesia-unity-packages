using UnityEngine;
using static UnityEngine.Mathf;

namespace Sonosthesia.Builder
{
    public static class FunctionLibrary
    {
        public enum FunctionName
        {
            Wave, 
            MultiWave, 
            Ripple,
            Sphere,
            PulsatingSphere,
            PerturbedSphere,
            Torus,
            PulsatingTorus,
            PerturbedTorus
        }

        public delegate Vector3 Function (float u, float v, float t);

        public static int Count() => functions.Length;
        
        private static readonly Function[] functions =
        {
            Wave, 
            MultiWave, 
            Ripple,
            Sphere,
            PulsatingSphere,
            PerturbedSphere,
            Torus,
            PulsatingTorus,
            PerturbedTorus
        };
        
        public static Function GetFunction (FunctionName name) => functions[(int)name];

        private static Vector3 Wave (float u, float v, float t) 
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + v + t));
            p.z = v;
            return p;
        }
        
        private static Vector3 MultiWave (float u, float v, float t) 
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + 0.5f * t));
            p.y += 0.5f * Sin(2f * PI * (v + t));
            p.y += Sin(PI * (u + v + 0.25f * t));
            p.y *= 1f / 2.5f;
            p.z = v;
            return p;
        }
        
        private static Vector3 Ripple (float u, float v, float t) 
        {
            float d = Sqrt(u * u + v * v);
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (4f * d - t));
            p.y /= 1f + 10f * d;
            p.z = v;
            return p;
        }
        
        private static Vector3 PulsatingSphere (float u, float v, float t) 
        {
            return Sphere(u, v, t, 0.5f + 0.5f * Sin(PI * t));
        }
        
        private static Vector3 PerturbedSphere (float u, float v, float t) 
        {
            return Sphere(u, v, t, 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t)));
        }
        
        private static Vector3 Sphere(float u, float v, float t)
        {
            return Sphere(u, v, t, 1f);
        }

        private static Vector3 Sphere(float u, float v, float t, float r)
        {
            float s = r * Cos(0.5f * PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r * Sin(0.5f * PI * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        
        private static Vector3 Torus(float u, float v, float t)
        {
            return Torus(u, v, t, 0.75f, 0.25f);
        }
        
        private static Vector3 PulsatingTorus(float u, float v, float t)
        {
            return Torus(u, v, t, 0.75f, 0.25f + 0.125f * Sin(PI * t));
        }
        
        private static Vector3 PerturbedTorus(float u, float v, float t)
        {
            float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
            float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
            return Torus(u, v, t, r1, r2);
        }
        
        
        private static Vector3 Torus(float u, float v, float t, float r1, float r2) 
        {
            float s = r1 + r2 * Cos(PI * v);
            Vector3 p;
            p.x = s * Sin(PI * u);
            p.y = r2 * Sin(PI * v);
            p.z = s * Cos(PI * u);
            return p;
        }

        public static Vector3 Morph(float u, float v, float t, Function from, Function to, float progress)
        {
            return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), SmoothStep(0f, 1f, progress));
        }
    }
}