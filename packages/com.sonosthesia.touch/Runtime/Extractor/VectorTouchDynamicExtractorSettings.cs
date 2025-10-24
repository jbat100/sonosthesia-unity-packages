using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Touch
{
    [Serializable]
    public class VectorTouchDynamicExtractorSettings : InteractionVectorDynamicExtractorSettings<TouchEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Direction,
            Velocity,
            Relative,
            Axis
        }
        
        [SerializeField] private ExtractorType _extractorType;
        
        protected override IDynamicExtractorSession<TouchEvent, Vector3> MakeRawSession()
        {
            IDynamicExtractorSession<TouchEvent, Vector3> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Velocity => VelocitySession(),
                ExtractorType.Direction => DirectionSession(),
                ExtractorType.Relative => RelativeSession(),
                ExtractorType.Axis => AxisSession(),
                _ => throw new ArgumentOutOfRangeException()
            };

            return session;
        }
    }
}