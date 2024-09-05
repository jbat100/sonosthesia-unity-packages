using System;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    // Intent based on scene switcher state
    
    public class DemoUIWindowSceneSwitch : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intent;

        [SerializeField] private SceneSwitcher _switcher;

        private IDisposable _subscription;

        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            if (_switcher)
            {
                _switcher.StateObservable.Subscribe(state =>
                {
                    if (state == SceneSwitcherState.FadeOut)
                    {
                        _intent.Broadcast(Intent.Empty);
                    }
                    else if (state == SceneSwitcherState.FadeIn && string.IsNullOrEmpty(_switcher.Current))
                    {
                        _intent.Broadcast(new Intent(UIDemoWindowIntentKeys.MAIN));
                    }
                });
            }
        }
    }
}