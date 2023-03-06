using UnityEngine;

namespace Sonosthesia.Utils
{
    public class Lerper : MonoBehaviour
    {
        [SerializeField] private float _lerp;

        public float Lerp
        {
            get => _lerp;
            set => _lerp = value;
        }
    }
}