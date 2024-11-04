using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Sonosthesia.Application
{
    public class ApplicationTestInteraction : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _targets;
        
        private ApplicationSettings _settings;
        
        [Inject]
        public void Construct(ApplicationSettings settings)
        {
            _settings = settings;
        }

        protected void OnEnable()
        {
            if (_settings == null)
            {
                return;
            }
            
            foreach (GameObject target in _targets)
            {
                target.SetActive(_settings.TestInteraction);
            }
        }
    }
}