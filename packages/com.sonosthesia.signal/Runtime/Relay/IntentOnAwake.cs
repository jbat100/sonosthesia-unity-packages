using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public class IntentOnAwake : MonoBehaviour
    {
        [SerializeField] private IntentSignalRelay _intent;

        [SerializeField] private string _key;

        [SerializeField] private float _delay;

        [SerializeField] private bool _mute;

        protected virtual void Awake()
        {
            Observable.Timer(TimeSpan.FromSeconds(Mathf.Max(0, _delay))).Subscribe(_ =>
            {
                if (_mute)
                {
                    return;
                }
                
                _intent.Broadcast(new Intent(_key));
            });
        }
    }
}