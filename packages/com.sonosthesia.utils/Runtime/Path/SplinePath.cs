using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Utils
{
    public class SplinePath : Path
    {
        [SerializeField] private SplineContainer _splineContainer;

        [SerializeField] private int _index;

        [SerializeField] private float _scaleFactor = 1f;

        private Spline Spline => _splineContainer[_index];
        
        protected override float ComputePathLength() => Spline.GetLength();

        protected virtual void OnEnable()
        {
            Spline.Changed += OnSplineChange;
        }

        protected virtual void OnDisable()
        {
            Spline.Changed -= OnSplineChange;
        }
        
        protected override Vector3 GetPosition(float normalized)
        {
            return Spline.EvaluatePosition(normalized) * _scaleFactor;
        }

        protected override Quaternion GetRotation(float normalized)
        {
            return Quaternion.LookRotation(
                Spline.EvaluateTangent(normalized),
                Spline.EvaluateUpVector(normalized));
        }

        protected override bool Evaluate(float normalized, out Vector3 position, out Quaternion rotation)
        {
            if (Spline.Evaluate(normalized, out float3 pos, out float3 tangent, out float3 up))
            {
                position = pos * _scaleFactor;
                rotation = Quaternion.LookRotation(tangent, up);
                return true;
            }
            position = default;
            rotation = default;
            return false;
        }
        
        private void OnSplineChange(Spline arg1, int arg2, SplineModification arg3)
        {
            ResetPathLength();
        }
    }
}