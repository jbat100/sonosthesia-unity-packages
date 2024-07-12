using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Application 
{
    public class SceneSwitcherUI : MonoBehaviour
    {
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

        protected virtual void Start()
        {
            if (!_switcher)
            {
                return;
            }
            
            foreach (Choice choice in _choices)
            {
                choice.Button.onClick.AddListener(() => _switcher.SwitchToScene(choice.Name));
            }            
        }
    }
}


