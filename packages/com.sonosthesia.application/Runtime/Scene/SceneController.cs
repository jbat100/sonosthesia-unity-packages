using System.Collections.Generic;
using System.Linq;
using Sonosthesia.Signal;
using UnityEngine;

namespace Sonosthesia
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private List<Signal<float>> _controls;

        [SerializeField] private List<GameObject> _instances;
        
        [SerializeField] private List<int> _indices = new List<int>() { 0 };
        public IReadOnlyList<int> Indices
        {
            get => _indices;
            set
            {
                _indices = value.ToList();
                ApplyIndices();
            }
        }

        public virtual string Info => "";

        public float GetControl(int index)
        {
            return _controls.Count > index ? _controls[index].Value : 0;
        }

        public void SetControl(int index, float value)
        {
            if (_controls.Count > index)
            {
                _controls[index].Broadcast(value);
            }
        }

        protected virtual void OnEnable()
        {
            ApplyIndices();
        }
        
        private void ApplyIndices()
        {
            for (int i = 0; i < _instances.Count; i++)
            {
                _instances[i].SetActive(_indices.Contains(i));
            }
        }
    }    
}


