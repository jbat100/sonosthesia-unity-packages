using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Sonosthesia.Application
{
    public class SceneSwitcherTest : MonoBehaviour
    {
        [SerializeField] private List<string> _scenes;

        private SceneSwitcher _switcher;
        private int _currentIndex = -1;

        [Inject]
        public void Construct(SceneSwitcher switcher)
        {
            _switcher = switcher;
        }
        
        public void Switch()
        {
            if (_scenes.Count == 0)
            {
                return;
            }
            _currentIndex = (_currentIndex + 1) % _scenes.Count;
            _switcher.SwitchToScene(_scenes[_currentIndex]).Forget();
        }
    }
}