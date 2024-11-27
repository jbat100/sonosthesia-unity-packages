using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Scaffold
{
    public class SplineGroupTransformer : GroupTransformer
    {
        [SerializeField] private SplineContainer _splineContainer;

        [SerializeField] private int _splineIndex;

        private enum SplineDirection
        {
            Tangent,
            InvertedTangent,
            Up,
            Down
        }

        [SerializeField] private SplineDirection _forwardDirection;
        
        [SerializeField] private SplineDirection _upDirection;

        
        protected override void OnEnable()
        {
            base.OnEnable();
            Spline.Changed += SplineOnChanged;
        }

        protected virtual void OnDisable()
        {
            Spline.Changed -= SplineOnChanged;
        }
        
        private void SplineOnChanged(Spline spline, int knot, SplineModification modification)
        {
            if (_splineContainer[_splineIndex] == spline)
            {
                BroadcastChange();
            }
        }

        public override void Apply<T>(IEnumerable<T> targets)
        {
            Spline spline = _splineContainer[_splineIndex];
            
            foreach (T element in targets)
            {
                if (spline.Evaluate(element.Offset, out float3 position, out float3 tangent, out float3 up))
                {
                    Vector3 SelectDirection(SplineDirection direction)
                    {
                        return direction switch
                        {
                            SplineDirection.Tangent => tangent,
                            SplineDirection.InvertedTangent => -tangent,
                            SplineDirection.Up => up,
                            SplineDirection.Down => -up,
                            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                        };
                    }
                    
                    element.Transform.localPosition = position;
                    element.Transform.localRotation = Quaternion.LookRotation(
                        SelectDirection(_forwardDirection), SelectDirection(_upDirection));
                }
            }
        }
    }
}