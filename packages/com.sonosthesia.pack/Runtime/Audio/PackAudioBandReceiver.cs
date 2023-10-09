using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackAudioBandReceiver : MonoBehaviour
    {
        [SerializeField] private AddressedPackConnection _connection;

        private CompositeDisposable _subscriptions = new ();
        
        private readonly Subject<PackedAudioTriBands> _band3Subject = new ();
        internal IObservable<PackedAudioTriBands> TriBandObservable 
            => _band3Subject.AsObservable();
        
        private readonly Subject<PackedAudioQuintBands> _band5Subject = new ();
        internal IObservable<PackedAudioQuintBands> QuintBandObservable 
            => _band5Subject.AsObservable();
        
        protected virtual void OnEnable()
        {
            _subscriptions.Clear();
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedAudioTriBands>(PackAudioAddress.BANDS_3)
                .ObserveOnMainThread()
                .Subscribe(_band3Subject));
            
            _subscriptions.Add(_connection.IncomingContentObservable<PackedAudioQuintBands>(PackAudioAddress.BANDS_5)
                .ObserveOnMainThread()
                .Subscribe(_band5Subject));
        }
        
        protected virtual void OnDisable() => _subscriptions.Clear();
    }
}