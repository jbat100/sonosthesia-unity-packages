using System;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public abstract class ScriptableRootedPool<T> : ScriptablePool<T> where T : class
    {
        [SerializeField, HideInInspector] private Guid _id = Guid.NewGuid();
        
        private Transform _root;
        /// <summary>
        /// Simple root to attach objects to
        /// </summary>
        protected Transform Root
        {
            get
            {
                if (_root)
                {
                    return _root;
                }

                GameObject rootObject = new GameObject($"ScriptablePool-{_id}");
                DontDestroyOnLoad(rootObject);
                _root = rootObject.transform;
                return _root;
            }
        }
    }
}