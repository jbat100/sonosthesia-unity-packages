using System;
using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorRelayController : MonoBehaviour
    {
        [SerializeField] private float _jumpSize = 15f;
        
        [SerializeField] private IntentSignalRelay _intent;
        
        [Header("State")]
        
        [SerializeField] private FloatSignalRelay _duration;

        [SerializeField] private FloatSignalRelay _time;

        [SerializeField] private BoolSignalRelay _playing;

        private PlayableDirector _playableDirector;
        private IDisposable _intentSubscription;

        protected virtual void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }
        
        protected virtual void OnEnable()
        {
            _intentSubscription?.Dispose();
            if (_intent && _playableDirector)
            {
                _intentSubscription = _intent.Observable.Subscribe(intent =>
                {
                    Debug.Log($"{this} process intent {intent}");
                    switch (intent.Key)
                    {
                        case ApplicationIntentKeys.TIME:
                            _playableDirector.SafeSetTime((float)intent.Payload);
                            break;
                        case ApplicationIntentKeys.JUMP:
                            _playableDirector.SafeJump(_jumpSize);
                            break;
                        case ApplicationIntentKeys.BACK:
                            _playableDirector.SafeJump(-_jumpSize);
                            break;
                        case ApplicationIntentKeys.TOGGLE:
                            if (_playableDirector.state == PlayState.Playing)
                            {
                                _playableDirector.Pause();    
                            }
                            else
                            {
                                _playableDirector.Play();
                            }
                            break;
                        case ApplicationIntentKeys.PLAY:
                            _playableDirector.Play();
                            break;
                        case ApplicationIntentKeys.PAUSE:
                            _playableDirector.Pause();
                            break;
                        case ApplicationIntentKeys.STOP:
                            _playableDirector.Pause();
                            _playableDirector.time = 0;
                            break;
                        case ApplicationIntentKeys.RESTART:
                            _playableDirector.time = 0;
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        protected virtual void OnDisable()
        {
            _intentSubscription?.Dispose();
        }

        protected virtual void Update()
        {
            if (!_playableDirector)
            {
                return;
            }
            
            if (_duration)
            {
                _duration.Broadcast((float)_playableDirector.duration);
            }

            if (_time)
            {
                _time.Broadcast((float)_playableDirector.time);
            }

            if (_playing)
            {
                _playing.Broadcast(_playableDirector.state == PlayState.Playing);
            }
        }
    }
}