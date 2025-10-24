using UnityEngine;

namespace Sonosthesia.Interaction
{
    public abstract class VectorStaticExtractorSettings<TEvent> : StaticExtractorSettings<TEvent, Vector3, VectorPostProcessingSettings>
    {

    }

    public abstract class InteractionVectorStaticExtractorSettings<TEvent> : VectorStaticExtractorSettings<TEvent>
        where TEvent : IInteractionEvent
    {
        [SerializeField] private VelocityExtractionType _velocityType;
        
        [SerializeField] private ExtractionSpace _space;

        [SerializeField] private Vector3 _direction;
        
        protected bool ExtractDirection(TEvent e, out Vector3 value) => e.ExtractDirection(_space, _direction, out value);
        protected bool ExtractVelocity(TEvent e, out Vector3 value) => e.ExtractVelocity(_velocityType, out value);
        protected bool ExtractRelative(TEvent e, out Vector3 value) => e.ExtractRelativePosition(out value);
        protected bool ExtractAxis(TEvent e, out Vector3 value) => e.ExtractAxis(out value);
    }
}