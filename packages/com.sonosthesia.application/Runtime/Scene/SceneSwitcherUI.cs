using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Sonosthesia.Application 
{
    public class SceneSwitcherUI : MonoBehaviour
    {
        [SerializeField] private Text _nameText;
        [SerializeField] private bool _lastNameComponentOnly;
        [SerializeField] private Text _stateText;

        [Serializable]
        public class Choice
        {
            [SerializeField] private Button _button;
            public Button Button => _button; 
                
            [SerializeField] private string _name;
            public string Name => _name;
        }
        
        [SerializeField] private Choice[] _choices;

        private CompositeDisposable _subscriptions = new ();
        
        private SceneSwitcher _switcher;
        
        [Inject]
        public void Construct(SceneSwitcher switcher)
        {
            _switcher = switcher;
        }
        
        protected virtual void Start()
        {
            foreach (Choice choice in _choices)
            {
                choice.Button.onClick.AddListener(() =>
                {
                    if (_switcher == null)
                    {
                        return;
                    }
                    _switcher.SwitchToScene(choice.Name).Forget();
                });
            }            
        }

        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            _subscriptions.Add(_switcher.Current.Subscribe(current =>
            {
                if (!_nameText)
                {
                    return;
                }
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
            }));
            _subscriptions.Add(_switcher.State.Subscribe(state =>
            {
                if (!_stateText)
                {
                    return;
                }
                _stateText.text = state.ToString();
            }));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
        
    }
}


