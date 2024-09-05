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

        protected virtual void Awake()
        {
            Observable.Timer(TimeSpan.FromSeconds(Mathf.Max(0, _delay))).Subscribe(_ =>
            {
                _intent.Broadcast(new Intent(_key));
            });
        }
    }
}