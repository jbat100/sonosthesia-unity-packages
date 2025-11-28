using System;
using Sonosthesia.Signal;
using Sonosthesia.AdaptiveMIDI;
using UnityEngine;
using UniRx;

namespace Sonosthesia.MIDI
{
    public class MIDIControlFloatSignal : StatelessSignal<float>
    {
        [SerializeField] private InterfaceReference<IMIDIMessageReceiver> _input;

        [SerializeField] private int _channel;

        [SerializeField] private int _number;

        private IDisposable _subscription;
        
        protected void Awake()
        {
            _input.Value ??= GetComponentInParent<IMIDIMessageReceiver>();
        }

        protected void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = _input.Value?.ControlObservable
                .Where(control => control.Channel == _channel && control.Number == _number)
                .Subscribe(control =>
                    {
                        Broadcast(control.Value / 127f);
                    });
        }

        protected void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}