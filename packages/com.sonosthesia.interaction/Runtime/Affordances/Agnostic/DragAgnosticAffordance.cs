using UnityEngine;

namespace Sonosthesia.Interaction
{
    public class DragAgnosticAffordance<TEvent> : AbstractAffordance<TEvent> where TEvent : struct, IInteractionEvent
    {
        // TODO : use pools 

        [SerializeField] private Transform _originPrefab;
        public Transform OriginPrefab => _originPrefab;
        
        [SerializeField] private Transform _targetPrefab;
        public Transform TargetPrefab => _targetPrefab;
        
        [SerializeField] private LineRenderer _lineRendererPrefab;
        public LineRenderer LineRendererPrefab => _lineRendererPrefab;
    }
}