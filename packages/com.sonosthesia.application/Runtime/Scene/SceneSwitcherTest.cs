using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sonosthesia.Application
{
    public class SceneSwitcherTest : MonoBehaviour
    {
        [SerializeField] private SceneSwitcher _switcher;
        
        [SerializeField] private List<string> _scenes;

        private int _currentIndex = -1;

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