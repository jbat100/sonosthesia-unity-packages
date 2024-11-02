using UnityEngine;

namespace Sonosthesia.Utils
{
    public abstract class Follower : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        public Transform Target => _target;
        
        public abstract void Align();
    }
}


