using UnityEngine;
using UnityEngine.Serialization;

namespace Sonosthesia.Utils
{
    public abstract class Follower : MonoBehaviour
    {
        [FormerlySerializedAs("target")] [SerializeField] private Transform _target;
        public Transform Target => _target;
        
        public abstract void Align();
    }
}


