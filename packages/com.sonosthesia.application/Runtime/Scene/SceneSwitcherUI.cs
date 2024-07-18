using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Application 
{
    public class SceneSwitcherUI : MonoBehaviour
    {
        [SerializeField] private Text _nameText;

        [SerializeField] private bool _lastNameComponentOnly;
        
        [SerializeField] private Text _stateText;

        [SerializeField] private SceneSwitcher _switcher;
        
        [Serializable]
        public class Choice
        {
            [SerializeField] private Button _button;
            public Button Button => _button; 
                
            [SerializeField] private string _name;
            public string Name => _name;
        }
        
        [SerializeField] private Choice[] _choices;

        private IDisposable _nameSubscription;
        
        protected virtual void Start()
        {
            if (!_switcher)
            {
                return;
            }
            
            foreach (Choice choice in _choices)
            {
                choice.Button.onClick.AddListener(() => _switcher.SwitchToScene(choice.Name).Forget());
            }            
        }

        protected virtual void OnEnable()
        {
            _nameSubscription?.Dispose();
            if (_switcher && _nameText)
            {
                _nameSubscription = _switcher.CurrentObservable.Subscribe(current =>
                {
                    if (_lastNameComponentOnly)
                    {
                        string[] components = current.Split("/");
                        if (components.Length > 0)
                        {
                            _nameText.text = components[^1];
                            return;
                        }
                    }

                    _nameText.text = current;
                });
            }
        }

        protected virtual void OnDisable() => _nameSubscription?.Dispose();
        
    }
}


