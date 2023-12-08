using UniRx;
using UnityEngine;

namespace Sonosthesia.AdaptiveMIDI
{
    public class RawMIDIInput : MIDIInput
    {
        [SerializeField] private RawMIDIInputStream _rawInput;

        private MIDIDecoder _decoder;
        private readonly CompositeDisposable _subscriptions = new ();
        
        protected virtual void OnEnable()
        {
            _decoder = new MIDIDecoder();
            
            _subscriptions.Clear();
            
            _subscriptions.Add(_rawInput.SingleObservable.Subscribe(raw => 
                _decoder.Decode(this, raw.Timestamp, raw.Data0)));
            
            _subscriptions.Add(_rawInput.DoubleObservable.Subscribe(raw => 
                _decoder.Decode(this, raw.Timestamp, raw.Data0, raw.Data1)));
            
            _subscriptions.Add(_rawInput.TrippleObservable.Subscribe(raw => 
                _decoder.Decode(this, raw.Timestamp, raw.Data0, raw.Data1, raw.Data2)));
        }

        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}