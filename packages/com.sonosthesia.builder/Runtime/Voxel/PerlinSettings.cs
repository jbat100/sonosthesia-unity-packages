using UnityEngine;
using UnityEngine.Events;

namespace Sonosthesia.Builder
{
    [CreateAssetMenu(fileName = "PerlinConfiguration", menuName = "Sonosthesia/Settings/PerlinConfiguration")]
    public class PerlinSettings : ScriptableObject
    {
        public int Octaves = 8;
        public float Scale = 0.001f;
        public float HeightScale = 10;
        public float HeightOffset = -33;
        public float Probability = 1;

        public UnityEvent OnChanged;

        protected void OnValidate() => OnChanged?.Invoke();

        public bool TryProbability() => Random.value < Probability;
    }
}