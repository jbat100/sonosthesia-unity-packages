using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Scaffold
{
    [CreateAssetMenu(fileName = "SphereTransformer", menuName = "Sonosthesia/Scaffold/SphereTransformer")]
    public class SphereTransformerConfiguration : GroupTransformerConfiguration
    {
        [SerializeField] private Vector3 _center = Vector3.zero;

        [SerializeField] private float _radius = 1f;

        private static Vector3[] FibonacciSphere(int count, float radius = 1f, Vector3? center = null)
        {
            Vector3 c = center ?? Vector3.zero;

            if (count <= 0) return Array.Empty<Vector3>();
            if (count == 1) return new[] { c + Vector3.up * radius };

            Vector3[] points = new Vector3[count];

            float goldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);       // 0..1
                float y = 1f - 2f * t;                  // 1..-1
                float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));

                float theta = goldenAngle * i;

                float x = Mathf.Cos(theta) * r;
                float z = Mathf.Sin(theta) * r;

                points[i] = c + new Vector3(x, y, z) * radius;
            }

            return points;
        }
        
        private static Vector3 SampleLinear(Vector3[] points, float t)
        {
            if (points == null || points.Length == 0)
                return Vector3.zero;

            if (points.Length == 1)
                return points[0];

            t = Mathf.Clamp01(t);

            float scaledIndex = t * (points.Length - 1);
            int indexA = Mathf.FloorToInt(scaledIndex);
            int indexB = Mathf.Min(indexA + 1, points.Length - 1);

            float localT = scaledIndex - indexA;

            return Vector3.Lerp(points[indexA], points[indexB], localT);
        }

        public override void Apply<T>(IReadOnlyList<T> targets)
        {
            int count = targets.Count;

            Vector3[] points = FibonacciSphere(count, _radius, Vector3.zero);

            foreach (T target in targets)
            {
                // note expect Offset [0,1]
                Vector3 sample = SampleLinear(points, target.Offset);
                Vector3 resized = sample.normalized * _radius;
                Quaternion rotation = Quaternion.LookRotation(resized, Vector3.up);
                target.Transform.localPosition = resized + _center;
                target.Transform.localRotation = rotation;
            }
        }
    }
}
