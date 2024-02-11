using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class RoundTripController : MonoBehaviour
    {
        [SerializeField] private PackEnvelopeHub _hub;

        [SerializeField] private float _period;

        [SerializeField] private int _groupSize = 1;

        [SerializeField] private int _bufferSize = 100;

        private IDisposable _timerSubscription;
        private IDisposable _responseSubscription;

        private CircularBuffer<float> _roundTripTimes = new(100);

        protected void OnEnable()
        {
            _responseSubscription = _hub
                .IncomingContentObservable<RoundTripResponse>(PackPerformanceAddress.ROUNDTRIP_RESPONSE)
                .Subscribe(response =>
                {
                    
                });
            
            _timerSubscription = Observable.Timer(TimeSpan.FromSeconds(_period)).Subscribe(_ =>
            {
                for (int i = 0; i < _groupSize; i++)
                {
                    RoundTripRequest request = new RoundTripRequest()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Timestamp = Time.time
                    };
                    _hub.QueueOutgoingContent(PackPerformanceAddress.ROUNDTRIP_REQUEST, request);
                }
            });
        }
        
        protected void OnDisable()
        {
            
        }
        
    }
}