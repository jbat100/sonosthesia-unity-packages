using System;
using Sonosthesia.Deform;
using Sonosthesia.Ease;
using UnityEngine;

namespace Sonosthesia.DeformInteraction
{
    // Noise component info should have center and direction
    // Noise zone should be :
    // - spherical : distance from point
    // - cylindrical : distance from line (with optional caps)
    // - planar : distance from plane
    
    // Center Tracking becomes Actor and Source tracking
    // Actor and source tracking should result in center and direction through FalloffStrategy:
    // - cylindrical actor source center
    // - cylindrical actor source axis
    // - spherical actor distance
    // - planar actor orientation
    // - planar actor source

    public enum TouchSpatialFalloffCenter
    {
        Actor,
        Source
    }
    
    public enum TouchSpatialFalloffSpace
    {
        World,
        Source,
        Actor,
        ActorSource,
    }
    
    [Serializable]
    public class TouchSpatialFalloffSettings
    {
        [SerializeField] private bool _active;
        public bool Active => _active;

        [SerializeField] private EaseType _easeType = EaseType.easeInOutSine;
        public EaseType EaseType => _easeType;
    
        [SerializeField] private TouchSpatialFalloffCenter _center = TouchSpatialFalloffCenter.Actor;
        public TouchSpatialFalloffCenter Center => _center;

        [SerializeField] private SpatialFalloffShape _shape = SpatialFalloffShape.Spherical;
        public SpatialFalloffShape Shape => _shape;
    
        [SerializeField] private TouchSpatialFalloffSpace _space = TouchSpatialFalloffSpace.Actor;
        public TouchSpatialFalloffSpace Space => _space;

        [SerializeField] private Vector3 offset = Vector3.up;
        public Vector3 Offset => offset;
    }
}