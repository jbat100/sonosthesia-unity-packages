using System;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;

namespace Sonosthesia
{
    public class DelayedPlay : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _playableDirector;

        [SerializeField] private float _start;
        
        [SerializeField] private float _delay;

        protected void OnEnable()
        {
            Observable.Timer(TimeSpan.FromSeconds(_delay)).Subscribe(_ =>
            {
                _playableDirector.time = _start;
                _playableDirector.Play();
            });
        }
    }    
}


