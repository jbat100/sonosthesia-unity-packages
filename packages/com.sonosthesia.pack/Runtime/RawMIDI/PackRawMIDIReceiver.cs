using System;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class PackRawMIDIReceiver : MonoBehaviour
    {
        [SerializeField] private AddressedPackConnection _connection;

        [SerializeField] private string _portFilter;

        private IDisposable _subscription;
        
        private readonly Subject<PackedRawMIDISourceSingle> _singleSubject = new ();
        internal IObservable<PackedRawMIDISourceSingle> SingleObservable 
            => _singleSubject.AsObservable();
        
        private readonly Subject<PackedRawMIDISourceDouble> _doubleSubject = new ();
        internal IObservable<PackedRawMIDISourceDouble> DoubleObservable 
            => _doubleSubject.AsObservable();
        
        private readonly Subject<PackedRawMIDISourceTripple> _trippleSubject = new ();
        internal IObservable<PackedRawMIDISourceTripple> TrippleObservable 
            => _trippleSubject.AsObservable();
        
        private protected virtual IDisposable Setup(AddressedPackConnection connection)
        {
            CompositeDisposable subscriptions = new();

            void Connect<T>(string address, IObserver<T> observer) where T : IPackedRawMIDISourceData
            {
                subscriptions.Add(connection.IncomingContentObservable<T>(address)
                    .Where(data => string.IsNullOrEmpty(_portFilter) || data.Port == _portFilter)
                    .ObserveOnMainThread()
                    .Subscribe(observer));
            }
            
            Connect(PackRawMIDISourceAddress.SINGLE, _singleSubject);
            Connect(PackRawMIDISourceAddress.DOUBLE, _doubleSubject);
            Connect(PackRawMIDISourceAddress.TRIPPLE, _trippleSubject);
            
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