using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Instrument
{
    [CreateAssetMenu(fileName = "ArcGroupTransfomerConfiguration", menuName = "Sonosthesia/Transformer/ArcGroupTransfomerConfiguration")]
    public class ArcGroupTransfomerConfiguration : GroupTransformerConfiguration
    {
        [SerializeField] private Vector3 _center;
        
        [SerializeField] private Vector3 _normal = Vector3.up;

        [SerializeField] private float _radius = 1f;

        [SerializeField] private float _start;

        [SerializeField] private float _end = 1f;

        private enum ArcDirection
        {
            Normal,
            InvertedNormal,
            Tangent,
            InvertedTangent,
            Out,
            In
        }

        [SerializeField] private ArcDirection _forwardDirection;
        
        [SerializeField] private ArcDirection _upDirection;

        [SerializeField] private Vector3 _rotationOffset;

        private const float TWO_PI = 2f * Mathf.PI;
        
        public override void Apply<T>(IEnumerable<T> targets)
        {
            float startAngle = _start * TWO_PI;
            float endAngle = _end * TWO_PI;
            float range = endAngle - startAngle;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, _normal);
            
            foreach (T element in targets)
            {
                float angle = startAngle + range * element.Offset;
                Vector3 position = (rotation * new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle))).normalized;

                Vector3 SelectDirection(ArcDirection direction)
                {
                    return direction switch
                    {
                        ArcDirection.Normal => _normal,
                        ArcDirection.InvertedNormal => -_normal,
                        ArcDirection.Tangent => Vector3.Cross(_normal, position),
                        ArcDirection.InvertedTangent => -Vector3.Cross(_normal, position),
                        ArcDirection.Out => position,
                        ArcDirection.In => -position,
                        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
                    };
                }

                element.Transform.localPosition = position * _radius;
                element.Transform.localRotation = Quaternion.LookRotation(
                    SelectDirection(_forwardDirection), SelectDirection(_upDirection)) 
                                                  * Quaternion.Euler(_rotationOffset);
            }
        }
    }
}