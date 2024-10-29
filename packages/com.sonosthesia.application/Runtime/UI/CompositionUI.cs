using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Sonosthesia.Application
{
    public class CompositionUI : MonoBehaviour
    {
        [SerializeField] private CompositionConfiguration _configuration;

        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _image;
        [SerializeField] private Button _launchButton;
        
        private SceneSwitcher _switcher;
        private IDisposable _buttonSubscription;
        
        [Inject]
        public void Construct(SceneSwitcher switcher)
        {
            _switcher = switcher;
        }

        public void Launch(string scenePath)
        {
            _switcher.SwitchToScene(scenePath).Forget();
        }

        protected void OnEnable()
        {
            _buttonSubscription?.Dispose();
            
            if (!_configuration)
            {
                return;
            }
            
            if (_descriptionText)
            {
                _descriptionText.text = _configuration.Description;   
            }

            if (_image)
            {
                _image.sprite = _configuration.Image;   
            }

            if (_launchButton)
            {
                _buttonSubscription = _launchButton.onClick.AsObservable()
                    .Subscribe(_ => Launch(_configuration.ScenePath));
            }
        }

        protected void OnDisable()
        {
            _buttonSubscription?.Dispose();
        }
    }
}