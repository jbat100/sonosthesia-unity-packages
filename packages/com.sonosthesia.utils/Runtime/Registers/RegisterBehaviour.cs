using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public class RegisterBehaviour<T> : MonoBehaviour
    {
        private readonly HashSet<T> _register = new();

        public bool RegisterValue(T value)
        {
            return _register.Add(value);
        }
        
        public bool UnregisterValue(T value)
        {
            return _register.Remove(value);
        }

        // avoids memory copy but breaks encapsulation
        protected IEnumerable<T> Raw => _register;
    }
}