using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackMIDIRawReceiver : MonoBehaviour
    {
        [SerializeField] private AddressedPackConnection _connection;

        private IDisposable _subscription;
        
        private readonly Subject<PackedMIDIRawSourceSingle> _singleSubject = new ();
        internal IObservable<PackedMIDIRawSourceSingle> SingleObservable 
            => _singleSubject.AsObservable();
        
        private readonly Subject<PackedMIDIRawSourceDouble> _doubleSubject = new ();
        internal IObservable<PackedMIDIRawSourceDouble> DoubleObservable 
            => _doubleSubject.AsObservable();
        
        private readonly Subject<PackedMIDIRawSourceTripple> _trippleSubject = new ();
        internal IObservable<PackedMIDIRawSourceTripple> TrippleObservable 
            => _trippleSubject.AsObservable();
        
        private protected virtual IDisposable Setup(AddressedPackConnection connection)
        {
            CompositeDisposable subscriptions = new();

            void Connect<T>(string address, IObserver<T> observer)
            {
                subscriptions.Add(connection.IncomingContentObservable<T>(address)
                    .ObserveOnMainThread()
                    .Subscribe(observer));
            }
            
            Connect(PackMIDIRawSourceAddress.SINGLE, _singleSubject);
            Connect(PackMIDIRawSourceAddress.DOUBLE, _doubleSubject);
            Connect(PackMIDIRawSourceAddress.TRIPPLE, _trippleSubject);
            
            return subscriptions;
        }
        
        protected virtual void OnEnable()
        {
            _subscription?.Dispose();
            _subscription = Setup(_connection);
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
            _subscription = null;
        } 
    }
}