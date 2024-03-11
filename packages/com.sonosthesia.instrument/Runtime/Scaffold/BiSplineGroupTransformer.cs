using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Instrument
{
    public class BiSplineGroupTransformer : GroupTransformer
    {
        [SerializeField] private SplineContainer _guideSplineContainer;

        [SerializeField] private int _guideSplineIndex;
        
        [SerializeField] private SplineContainer _orientationSplineContainer;

        [SerializeField] private int _orientationSplineIndex;

        [SerializeField] private Vector3 _scale = Vector3.one;

        [SerializeField] private Axes _growAxes = Axes.Y;
        
        private enum BiSplineDirection
        {
            GuideTangent,
            InvertedGuideTangent,
            GuideUp,
            GuideDown,
            GuideRight,
            GuideLeft,
            Orientation,
            InvertedOrientation,
            CrossOrientation,
            InvertedCrossOrientation
        }

        [SerializeField] private BiSplineDirection _forwardDirection = BiSplineDirection.GuideTangent;
        
        [SerializeField] private BiSplineDirection _upDirection = BiSplineDirection.Orientation;
        
        private Spline GuideSpline => _guideSplineContainer[_guideSplineIndex];
        
        private Spline OrientationSpline => _orientationSplineContainer[_orientationSplineIndex];
        
        protected virtual void Awake()
        {
            Spline.Changed += SplineOnChanged;
        }

        protected virtual void OnDestroy()
        {
            Spline.Changed -= SplineOnChanged;
        }
        
        private void SplineOnChanged(Spline spline, int knot, SplineModification modification)
        {
            Debug.Log($"{this} {nameof(SplineOnChanged)}");
            if (GuideSpline == spline || OrientationSpline == spline)
            {
                BroadcastChange();
            }
        }
        
        public override void Apply<T>(IEnumerable<T> targets)
        {
            Spline guideSpline = GuideSpline;
            Spline orientationSpline = OrientationSpline;
            
            foreach (T element in targets)
            {
                if (!guideSpline.Evaluate(element.Offset, out float3 guidePosition, out float3 guideTangent, out float3 guideUp))
                {
                    Debug.LogError($"{this} could not evaluate guide position for {element} with offset {element.Offset}");
                    continue;
                }

                if (guideTangent.Equals(float3.zero) || guideUp.Equals(float3.zero))
                {
                    Debug.LogError($"{this} evaluated to zero, this may occur when knots are on [0,1] [0,0], looks like a bug");
                    guideSpline.Evaluate(element.Offset, out guidePosition, out guideTangent, out guideUp);
                }

                float3 orientationPosition = orientationSpline.EvaluatePosition(element.Offset);

                Vector3 SelectDirection(BiSplineDirection direction)
                {
                    return direction switch
                    {
                        BiSplineDirection.GuideTangent 
                            => guideTangent,
                        BiSplineDirection.GuideUp 
                            => guideUp,
                        BiSplineDirection.GuideRight 
                            => Vector3.Cross(guideTangent, guideUp),
                        BiSplineDirection.Orientation 
                            => Vector3.Normalize(orientationPosition - guidePosition),
                        BiSplineDirection.CrossOrientation 
                            => Vector3.Cross(guideTangent, Vector3.Normalize(orientationPosition - guidePosition)),
                        BiSplineDirection.InvertedGuideTangent 
                            => -guideTangent,
                        BiSplineDirection.GuideDown 
                            => -guideUp,
                        BiSplineDirection.GuideLeft 
                            => -Vector3.Cross(guideTangent, guideUp),
                        BiSplineDirection.InvertedOrientation 
                            => -Vector3.Normalize(orientationPosition - guidePosition),
                        BiSplineDirection.InvertedCrossOrientation 
                            => -Vector3.Cross(guideTangent, Vector3.Normalize(orientationPosition - guidePosition)),
                        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                    };
                }

                element.Transform.localPosition = guidePosition;

                Vector3 forward = SelectDirection(_forwardDirection);
                Vector3 up = SelectDirection(_upDirection);

                element.Transform.localRotation = Quaternion.LookRotation(forward, up);

                Vector3 localScale = Vector3.Magnitude(guidePosition - orientationPosition) * _scale;
                
                element.ScaleTransform.localScale = element.ScaleTransform.localScale.SetAxes(localScale, _growAxes);
                element.ScaleTransform.localScale = element.ScaleTransform.localScale.SetAxes(_scale, ~_growAxes);
            }
        }
    }
}