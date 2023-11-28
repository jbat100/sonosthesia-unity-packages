using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.Pack
{
    public class PackRawMIDIInput : MIDIInput
    {
        [SerializeField] private PackRawMIDIReceiver _receiver;

        private readonly CompositeDisposable _subscriptions = new();

        private readonly MIDIDecoder _decoder = new();
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            _subscriptions.Add(_receiver.SingleObservable.Subscribe(data =>
            {
                _decoder.Decode(this, data.Timestamp(), data.B0);
            }));
            _subscriptions.Add(_receiver.DoubleObservable.Subscribe(data =>
            {
                _decoder.Decode(this, data.Timestamp(), data.B0, data.B1);
            }));
            _subscriptions.Add(_receiver.TrippleObservable.Subscribe(data =>
            {
                _decoder.Decode(this, data.Timestamp(), data.B0, data.B1, data.B2);
            }));
        }
        
        protected virtual void OnDisable()
        {
            _subscriptions.Clear();
        }
    }
}