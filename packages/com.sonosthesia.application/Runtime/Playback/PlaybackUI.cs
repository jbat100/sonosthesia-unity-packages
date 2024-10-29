using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

namespace Sonosthesia.Application
{
    public class PlaybackUI : MonoBehaviour
    {
        [SerializeField] private PlayableDirectorSlot _playableDirectorSlot;

        [SerializeField] private float _jumpTime = 10f;

        [Header("Interface")] 
        
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _jumpButton;
        [SerializeField] private Button _backButton;

        [SerializeField] private Slider _positionSlider;

        private readonly CompositeDisposable _subscriptions = new ();
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            
            _subscriptions.Add(_playButton.onClick.AsObservable()
                .Subscribe(_ => SafeAction(d => d.Play())));
            
            _subscriptions.Add(_pauseButton.onClick.AsObservable()
                .Subscribe(_ => SafeAction(d => d.Pause())));
            
            _subscriptions.Add(_stopButton.onClick.AsObservable()
                .Subscribe(_ => SafeAction(d =>
                {
                    d.Pause();
                    d.time = 0f;
                })));
            
            _subscriptions.Add(_jumpButton.onClick.AsObservable()
                .Subscribe(_ => SafeAction(d => d.SafeJump(_jumpTime))));
            
            _subscriptions.Add(_backButton.onClick.AsObservable()
                .Subscribe(_ => SafeAction(d => d.SafeJump(-_jumpTime))));

            _positionSlider.onValueChanged.AsObservable()
                .Subscribe(time => SafeAction(d => d.SafeSetTime(time)));
        }

        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
        }

        protected virtual void Update()
        {
            PlayableDirector director = Director;

            bool isPlaying = director && director.state == PlayState.Playing;
            float duration = (float)(director ? director.duration : 1);
            float position = (float)(director ? director.time : 0);

            if (_playButton)
            {
                _playButton.gameObject.SetActive(!isPlaying);
            }

            if (_pauseButton)
            {
                _pauseButton.gameObject.SetActive(isPlaying);
            }

            if (_positionSlider)
            {
                _positionSlider.minValue = 0;
                _positionSlider.maxValue = duration;
                _positionSlider.SetValueWithoutNotify(position);
            }
        }

        private PlayableDirector Director => !_playableDirectorSlot ? null : _playableDirectorSlot.Value;

        private void SafeAction(Action<PlayableDirector> action)
        {
            if (_playableDirectorSlot.Value)
            {
                action(_playableDirectorSlot.Value);
            }
        }
    }
}