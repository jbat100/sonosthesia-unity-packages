using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
    [Serializable]
    public class ChannelSequenceElement
    {
        [SerializeField] private float _delay;
        public float Delay => _delay;

        [SerializeField] private float _duration;
        public float Duration => _duration;
    }
    
    public abstract class ChannelSequencer<T, TElement> : MonoBehaviour where T : struct where TElement : ChannelSequenceElement
    {
        [SerializeField] private Channel<T> _target;

        [SerializeField] private bool _autoPlay;
        
        [SerializeField] private bool _loop;
        
        [SerializeField] private bool _offEvent;

        [SerializeField] private List<TElement> _elements;
        
        protected virtual void Sequence(IObservable<T> stream)
        {
            if (_target)
            {
                _target.Pipe(stream);    
            }
        }

        protected abstract T ForgeStart(TElement element);
        
        protected abstract T ForgeEnd(TElement element);
        
        private CancellationTokenSource _cancellationTokenSource;

        protected virtual void Start()
        {
            if (_autoPlay)
            {
                Play();
            }
        }
        
        public void Play()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            PlaySequence(_cancellationTokenSource.Token).Forget();
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
        
        private async UniTask PlaySequence(CancellationToken cancellationToken)
        {
            int index = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (index >= _elements.Count)
                {
                    if (_loop)
                    {
                        index = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                await PlayElement(_elements[index], cancellationToken);
                index++;
            }
        }

        private bool HOT = false;
        
        private async UniTask PlayElement(TElement sequenceEvent, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(sequenceEvent.Delay), cancellationToken: cancellationToken);
            T start = ForgeStart(sequenceEvent);
            IObservable<T> stream = Observable.Return(start);
            if (_offEvent)
            {
                T end = ForgeEnd(sequenceEvent);
                stream = stream.Concat(Observable.Timer(TimeSpan.FromSeconds(sequenceEvent.Duration))
                    .Select(_ => end));
            }
            else
            {
                stream = stream.Concat(Observable.Empty<T>()
                    .Delay(TimeSpan.FromSeconds(sequenceEvent.Duration)));
            }

            if (HOT)
            {
                // we want a hot observable which starts immediately after Sequence 
                IConnectableObservable<T> connected = stream.Publish();
                Sequence(connected);
                UniTask task = stream.ToUniTask(cancellationToken: cancellationToken);
                connected.Connect();
                await task;    
            }
            
            Sequence(stream);
            await stream.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}