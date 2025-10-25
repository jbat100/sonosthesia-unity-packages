using System;
using Sonosthesia.Interaction;
using UnityEngine;

namespace Sonosthesia.Collide
{
    [Serializable]
    public class VectorCollideDynamicExtractorSettings : InteractionVectorDynamicExtractorSettings<CollideEvent>
    {
        public enum ExtractorType
        {
            Custom,
            Constant,
            Direction,
            Velocity,
            Relative,
            Axis,
            ContactPoint,
            ContactNormal,
        }
        
        [SerializeField] private ExtractorType _extractorType;
        
        protected override IDynamicExtractorSession<CollideEvent, Vector3> MakeRawSession()
        {
            IDynamicExtractorSession<CollideEvent, Vector3> session = _extractorType switch
            {
                ExtractorType.Custom => CustomSession(),
                ExtractorType.Constant => ConstantSession(),
                ExtractorType.Velocity => VelocitySession(),
                ExtractorType.Direction => DirectionSession(),
                ExtractorType.Relative => RelativeSession(),
                ExtractorType.Axis => AxisSession(),
                ExtractorType.ContactPoint => new ContactPointVectorExtractorSession(Space),
                ExtractorType.ContactNormal => new ContactNormalVectorExtractorSession(Space),
                _ => throw new ArgumentOutOfRangeException()
            };

            return session;
        }
        
        private class ContactPointVectorExtractorSession : StatelessExtractorSession<CollideEvent, Vector3>
        {
            private readonly ExtractionSpace _space;
            
            public ContactPointVectorExtractorSession(ExtractionSpace space)
            {
                _space = space;
            }
            
            protected override bool Extract(CollideEvent e, out Vector3 value) => e.ExtractContactPoint(_space, out value);
        }
        
        private class ContactNormalVectorExtractorSession : StatelessExtractorSession<CollideEvent, Vector3>
        {
            private readonly ExtractionSpace _space;
            
            public ContactNormalVectorExtractorSession(ExtractionSpace space)
            {
                _space = space;
            }
            
            protected override bool Extract(CollideEvent e, out Vector3 value) => e.ExtractContactNormal(_space, out value);
        }
    }
}