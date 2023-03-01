using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Sonosthesia.Spawn
{
    
    /// <summary>
    /// TransSplineModulator from SplineAnimate.cs 
    /// </summary>
    public class RigidTransformSplineModulator : RigidTransformModulator
    {
        
        /// <summary>
        /// Describes the ways the object can be aligned when animating along the spline.
        /// </summary>
        private enum AlignmentMode
        {
            /// <summary> No aligment is done and object's rotation is unaffected. </summary>
            [InspectorName("None")]
            None,
            /// <summary> The object's forward and up axes align to the spline's tangent and up vectors. </summary>
            [InspectorName("Spline Element")]
            SplineElement,
            /// <summary> The object's forward and up axes align to the spline tranform's z-axis and y-axis. </summary>
            [InspectorName("Spline Object")]
            SplineObject,
            /// <summary> The object's forward and up axes align to to the world's z-axis and y-axis. </summary>
            [InspectorName("World Space")]
            World
        }
        
        /// <summary>
        /// Describes the different types of object alignment axes.
        /// </summary>
        private enum AlignAxis
        {
            /// <summary> Object space X axis. </summary>
            [InspectorName("Object X+")]
            XAxis,
            /// <summary> Object space Y axis. </summary>
            [InspectorName("Object Y+")]
            YAxis,
            /// <summary> Object space Z axis. </summary>
            [InspectorName("Object Z+")]
            ZAxis,
            /// <summary> Object space negative X axis. </summary>
            [InspectorName("Object X-")]
            NegativeXAxis,
            /// <summary> Object space negative Y axis. </summary>
            [InspectorName("Object Y-")]
            NegativeYAxis,
            /// <summary> Object space negative Z axis. </summary>
            [InspectorName("Object Z-")]
            NegativeZAxis
        }
                
        readonly float3[] m_AlignAxisToVector = new float3[] {math.right(), math.up(), math.forward(), math.left(), math.down(), math.back()};

        [SerializeField, Tooltip("Which axis of the GameObject is treated as the forward axis.")]
        private AlignAxis _forwardAxis = AlignAxis.ZAxis;
        
        [SerializeField, Tooltip("Which axis of the GameObject is treated as the up axis.")]
        private AlignAxis _upAxis = AlignAxis.YAxis;
        
        [SerializeField] private SplineContainer _target;

        [SerializeField] private AlignmentMode _alignmentMode;
        
        [SerializeField] private AnimationCurve _modulation;
        
        private SplinePath<Spline> _splinePath;
        
        protected override RigidTransform Modulation(float offset)
        {
            float t = _modulation.Evaluate(offset);
            RigidTransform rigidTransform = EvaluateRigidTransform(t);
            return rigidTransform;
        }
        
        private float3 GetAxis(AlignAxis axis)
        {
            return m_AlignAxisToVector[(int) axis];
        }
        
        private RigidTransform EvaluateRigidTransform(float t)
        {
            _splinePath ??= new SplinePath<Spline>(_target.Splines);
            
            float3 position = _target.EvaluatePosition(_splinePath, t);
            quaternion rotation;

            // Correct forward and up vectors based on axis remapping parameters
            float3 remappedForward = GetAxis(_forwardAxis);
            float3 remappedUp = GetAxis(_upAxis);
            Quaternion axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

            if (_alignmentMode != AlignmentMode.None)
            {
                Vector3 forward = Vector3.forward;
                Vector3 up = Vector3.up;

                switch (_alignmentMode)
                {
                    case AlignmentMode.SplineElement:
                        forward = Vector3.Normalize(_target.EvaluateTangent(_splinePath, t));
                        up = _target.EvaluateUpVector(_splinePath, t);
                        break;

                    case AlignmentMode.SplineObject:
                        var objectRotation = _target.transform.rotation;
                        forward = objectRotation * forward;
                        up = objectRotation * up;
                        break;

                    default:
                        Debug.Log($"{_alignmentMode} animation aligment mode is not supported!");
                        break;
                }

                rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;
            }
            else
            {
                rotation = axisRemapRotation;
            }

            return new RigidTransform(rotation, position);
        }
    }
}