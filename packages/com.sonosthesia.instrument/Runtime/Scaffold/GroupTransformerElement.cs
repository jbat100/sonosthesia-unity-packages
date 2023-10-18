using UnityEngine;

namespace Sonosthesia.Instrument
{
    public interface IGroupTransformerElement : IIndexed
    {
        Transform Transform { get; }
        
        float Offset { get; set; }
    }
    
    public class GroupTransformerElement : MonoBehaviour, IGroupTransformerElement
    {
        public Transform Transform => transform;
        
        public float Offset { get; set; }
        
        public int Index { get; set; }
    }
}