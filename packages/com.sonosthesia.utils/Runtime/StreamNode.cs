using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Sonosthesia.Utils
{
    
    /// <summary>
    /// Rx purists would probably be unhappy, facilitates observing a stream of streams and getting current values
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class StreamNode<TValue> : IDisposable where TValue : struct
    {
        private readonly ReactiveDictionary<Guid, TValue> _values = new();
        public IReadOnlyReactiveDictionary<Guid, TValue> Values => _values;

        private readonly Subject<IObservable<TValue>> _streamSubject = new();
        public IObservable<IObservable<TValue>> StreamObservable => _streamSubject.AsObservable();

        private readonly IDisposable _disableSubscription;
        private Component _component;
        public StreamNode(Component component)
        {
            _component = component;
            _disableSubscription = _component.OnDisableAsObservable().Subscribe(_ => _values.Clear());
        }

        public StreamNode()
        {
            _component = null;
        }

        public void Pipe(Guid id, IObservable<TValue> stream)
        {
            if (_component)
            {
                stream = stream.TakeUntilDisable(_component);
            }

            stream.Subscribe(
                valueEvent => _values[id] = valueEvent, 
                error => _values.Remove(id),
                () => _values.Remove(id)
            );
            
            _streamSubject.OnNext(stream);
        }

        public void Dispose()
        {
            _component = null;
            _disableSubscription?.Dispose();
            _values?.Dispose();
            _streamSubject?.OnCompleted();
            _streamSubject?.Dispose();
        }
    }
}