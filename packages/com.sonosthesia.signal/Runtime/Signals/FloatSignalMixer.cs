using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public enum FloatSignalMixerMode
    {
        None,
        Add,
        Max,
        Min
    }
    
    public class FloatSignalMixer : Signal<float>
    {
        [SerializeField] private FloatSignalMixerMode _mode;
        
        [SerializeField] private List<Signal<float>> _inputs = new ();

        private readonly Dictionary<Signal<float>, float> _values = new ();

        private readonly CompositeDisposable _subscriptions = new ();

        protected void OnEnable()
        {
            _subscriptions.Clear();
            _values.Clear();
            foreach (Signal<float> signal in _inputs)
            {
                _subscriptions.Add(signal.SignalObservable.Subscribe(value =>
                {
                    _values[signal] = value;
                    Broadcast(Compute());
                }));
            }
        }
        
        protected void OnDisable()
        {
            _subscriptions.Clear();
            _values.Clear();
        }

        private float Compute()
        {
            return _mode switch
            {
                FloatSignalMixerMode.Add => _values.Values.Sum(),
                FloatSignalMixerMode.Max => _values.Values.Max(),
                FloatSignalMixerMode.Min => _values.Values.Min(),
                _ => 0f
            };
        }
    }
}