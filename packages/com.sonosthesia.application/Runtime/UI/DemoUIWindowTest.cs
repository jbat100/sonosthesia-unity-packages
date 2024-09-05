using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Application
{
    public class DemoUIWindowTest : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intent;

        [SerializeField] private StateSignalRelay _currentScene;

        public void Broadcast(string key)
        {
            if (_intent)
            {
                _intent.Broadcast(new Intent(key, null));
            }
        }

        public void AdaptiveBroadcast()
        {
            if (_currentScene && !string.IsNullOrEmpty(_currentScene.Value.Key))
            {
                Broadcast(UIDemoWindowIntentKeys.SCENE);
            }
            else
            {
                Broadcast(UIDemoWindowIntentKeys.MAIN);
            }
        }
    }
}