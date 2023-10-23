using UnityEngine;

namespace Sonosthesia.Instrument
{
    public interface IGroupTransformerElement : IIndexed
    {
        Transform Transform { get; }
        
        Transform ScaleTransform { get; }
        
        float Offset { get; set; }
    }
    
    public class GroupTransformerElement : MonoBehaviour, IGroupTransformerElement
    {
        [SerializeField] private Transform _scaleTarget;
        
        public Transform Transform => transform;

        public Transform ScaleTransform => _scaleTarget ? _scaleTarget : transform;
        
        public float Offset { get; set; }
        
        public int Index { get; set; }
    }
}