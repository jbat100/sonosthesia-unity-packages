using UnityEngine;

namespace Sonosthesia.Signal
{
    public class Operator<T> : Adaptor<T, T> where T : struct
    {
        [SerializeField] private bool _bypass;
        protected bool Bypass => _bypass;
    }
}