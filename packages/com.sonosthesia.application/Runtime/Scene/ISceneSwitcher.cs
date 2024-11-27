using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Application
{
    public enum SceneSwitcherState
    {
        Empty,
        FadeOut,
        Unloading,
        Loading,
        FadeIn,
        Idle
    }

    public readonly struct SceneSwitcherFade
    {
        public readonly bool In;
        public readonly float Duration;

        public SceneSwitcherFade(bool fadeIn, float duration = 0f)
        {
            In = fadeIn;
            Duration = duration;
        }
    }

    [Serializable]
    public class SceneSwitcherSettings
    {
        [SerializeField] private float _fadeIn = 1f;
        public float FadeIn => _fadeIn;
        
        [SerializeField] private float _fadeOut = 1f;
        public float FadeOut => _fadeOut;

        [SerializeField] private bool _hideUI = true;
        public bool HideUI => _hideUI;
    }
    
    public interface ISceneSwitcher
    {
        IReadOnlyReactiveProperty<SceneSwitcherState> State { get; }

        public IReadOnlyReactiveProperty<string> Current { get; }
    }
}