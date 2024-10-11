using System;
using UnityEngine;

namespace Sonosthesia.Deform
{
    public class CompoundNoisePathProcessorComponent : MonoBehaviour
    {
        [Serializable]
        public class Settings
        {
            [SerializeField] private float _displacement = 0.1f;
            [SerializeField] private float _scale = 1f;
            [SerializeField] private Vector3 _offset;
            [SerializeField] private Vector3 _direction = Vector3.up;
            [SerializeField] private float _speed = 1f;
        }
    }
}