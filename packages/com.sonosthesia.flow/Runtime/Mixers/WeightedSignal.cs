using UnityEngine;
using Sonosthesia.Signal;

namespace Sonosthesia.Flow
{
    public class WeightedSignal<T> : MonoBehaviour where T : struct
    {
        [SerializeField] private StatelessSignal<T> _signal;
        public StatelessSignal<T> Signal => _signal;
        
        [SerializeField] private float _weight;
        public float Weight => _weight;
    }
}