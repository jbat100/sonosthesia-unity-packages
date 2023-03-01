using System;
using Sonosthesia.Flow;
using Sonosthesia.AdaptiveMIDI;
using UnityEngine;
using UniRx;

namespace Sonosthesia.MIDI
{
    public class MIDIControlSignal : Signal<float>
    {
        [SerializeField] private MIDIInput _input;

        [SerializeField] private int _channel;

        [SerializeField] private int _number;

        private IDisposable _subscription;
        
        protected void Awake()
        {
            if (!_input)
            {
                _input = GetComponentInParent<MIDIInput>();
            }
        }

        protected void OnEnable()
        {
            _subscription?.Dispose();
            if (!_input)
            {
                return;
            }
            _subscription = _input.ControlObservable
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