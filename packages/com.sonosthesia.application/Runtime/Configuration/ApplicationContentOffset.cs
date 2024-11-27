using UnityEngine;
using VContainer;

namespace Sonosthesia.Application
{
    public class ApplicationContentOffset : MonoBehaviour
    {
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
            
            transform.position = _settings.ContentOffset;
        }
    }
}