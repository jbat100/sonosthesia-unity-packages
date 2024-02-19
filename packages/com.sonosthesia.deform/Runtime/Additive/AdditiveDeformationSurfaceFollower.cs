using Sonosthesia.Noise;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Deform
{
    /// <summary>
    /// Controls a target transform so that it appears to float on an additively deformed mesh 
    /// </summary>
    public class AdditiveDeformationSurfaceFollower : MonoBehaviour
    {
        private const float RADIUS = 1f;
        
        // not sure why but seems to be off by a factor of 2
        private const float CORRECTION = 0.5f;
        
        [SerializeField] private AdditiveDeformationMeshController _meshController;

        [Tooltip("Transform forward gives direction for surface position calculation")]
        [SerializeField] private Transform _direction;

        [Tooltip("Transform which will be updated to mimic surface floating")]
        [SerializeField] private Transform _target;

        protected virtual void Update()
        {
            Vector3 worldDirection = _direction.forward;
            Vector3 meshDirection = _meshController.transform.InverseTransformDirection(worldDirection).normalized;
            Vector3 meshPosition = meshDirection * RADIUS;
            
            // we don't have single point deformation compute so we go through a redundant 4 wide
            float3x4 vertex = new float3x4(meshPosition, meshPosition, meshPosition, meshPosition);
            Sample4 deformation = _meshController.ComputeDeformation(vertex);
            Vector3 deformedMeshPosition = meshPosition + deformation.v.x * CORRECTION * meshDirection;

            Vector3 worldPosition = _meshController.transform.TransformPoint(deformedMeshPosition);
            
            Quaternion worldRotation = Quaternion.LookRotation(worldDirection, Vector3.up);
            
            transform.position = _meshController.transform.TransformPoint(meshPosition);
            transform.rotation = worldRotation;
            
            _target.position = _meshController.transform.TransformPoint(deformedMeshPosition);
            _target.rotation = worldRotation;
        }
    }
}