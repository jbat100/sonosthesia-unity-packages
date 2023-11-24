using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.RtMIDI
{
    public class RtMIDIInput : MIDIInput
    {
        public enum PollMode
        {
            Update,
            IntervalThread,
            IntervalTask
        }
        
        [SerializeField] private string _portName = "IAC Driver Unity";

        [SerializeField] private float _retryInterval = 1;

        [SerializeField] private PollMode _pollMode;
        
        [SerializeField] private float _pollInterval = 0.005f;

        private float PollInterval => Mathf.Max(_pollInterval, 0.001f);
        
        private RtMIDIInputProbe _probe;
        private Poller _poller;
        private IDisposable _subscription;

        private abstract class Poller : IDisposable
        {
            public Poller(RtMIDIInputPort port) { }

            public abstract void Dispose();
        }

        private class UpdatePoller : Poller
        {
            private readonly IDisposable _subscription;
            private readonly RtMIDIInputPort _port;
            
            public UpdatePoller(RtMIDIInputPort port) : base(port)
            {
                _port = port;
                _subscription = Observable.EveryUpdate().Subscribe(_ =>
                {
                    _port?.ProcessMessageQueue();
                });
            }

            public override void Dispose()
            {
                _subscription?.Dispose();
                _port?.Dispose();
            }
        }

        private class TaskPoller : Poller
        {
            private RtMIDIInputPort _port;
            
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly Task _pollingTask;
            private readonly object _lock = new ();
            
            public TaskPoller(TimeSpan interval, RtMIDIInputPort port) : base(port)
            {
                _port = port;
                _pollingTask = Task.Run(() => Poll(interval, _cancellationTokenSource.Token)); 
            }
            
            private async Task Poll(TimeSpan interval, CancellationToken token)
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        lock (_lock)
                        {
                            _port?.ProcessMessageQueue();
                        }
                        await Task.Delay(interval, token);
                    }
                    catch (Exception)
                    {
                        // Handle the cancellation
                    }
                }
            }

            public override void Dispose()
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                lock (_lock)
                {
                    _port?.Dispose();
                    _port = null;
                }
            }
        }

        private class ThreadPoller : Poller
        {
            private RtMIDIInputPort _port;
            
            private readonly CancellationTokenSource _cancellationTokenSource = new();
            private readonly Thread _pollingThread;
            private readonly object _lock = new ();
            
            public ThreadPoller(TimeSpan interval, RtMIDIInputPort port) : base(port)
            {
                _port = port;
                CancellationToken token = _cancellationTokenSource.Token;
                _pollingThread = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        lock (_lock)
                        {
                            try
                            {
                                _port.ProcessMessageQueue();
                            }
                            catch (Exception e)
                            {
                                // silent
                            }
                            
                        }
                        Thread.Sleep(interval);
                    }
                });
            }
            
            public override void Dispose()
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                lock (_lock)
                {
                    _port?.Dispose();
                    _port = null;
                }
            }
        }

        private string Description()
        {
            StringBuilder builder = new StringBuilder("MIDIInput scan : ");
            int count = _probe.PortCount;
            for (int i = 0; i < count; i++)
            {
                builder.Append(_probe.GetPortName(i) + ", ");
            }
            return builder.ToString();
        }
        
        private void Scan()
        {
            _subscription?.Dispose();
            _subscription = Observable.Interval(TimeSpan.FromSeconds(Mathf.Max(1f, _retryInterval))).Subscribe(_ =>
            {
                Debug.Log(Description());
                int count = _probe.PortCount;
                bool found = false;
                for (int i = 0; i < count; i++)
                {
                    string portName = _probe.GetPortName(i);
                    if (portName == _portName)
                    {
                        found = true;
                        _subscription?.Dispose();
                        _poller?.Dispose();
                        
                        RtMIDIInputPort port = new RtMIDIInputPort(i, portName);
                        port.NoteOnObservable.ObserveOnMainThread().Subscribe(BroadcastNoteOn);
                        port.NoteOffObservable.ObserveOnMainThread().Subscribe(BroadcastNoteOff);
                        port.ControlObservable.ObserveOnMainThread().Subscribe(BroadcastControl);
                        port.ChannelAftertouchObservable.ObserveOnMainThread().Subscribe(BroadcastChannelAftertouch);
                        port.PolyphonicAftertouchObservable.ObserveOnMainThread().Subscribe(BroadcastPolyphonicAftertouch);
                        port.PitchBendObservable.ObserveOnMainThread().Subscribe(BroadcastPitchBend);
                        port.SongPositionPointerObservable.ObserveOnMainThread().Subscribe(BroadcastPositionPointer);
                        port.SyncObservable.ObserveOnMainThread().Subscribe(BroadcastSync);
                        port.ClockObservable.ObserveOnMainThread().Subscribe(BroadcastClock);

                        // NOTE: now that we have the timestamps comming directly from RtMidi, it is debatable
                        // whether the thread polling is actually useful, check performance
                        
                        _poller = _pollMode switch
                        {
                            PollMode.Update => new UpdatePoller(port),
                            PollMode.IntervalThread => new ThreadPoller(TimeSpan.FromSeconds(PollInterval), port),
                            PollMode.IntervalTask => new TaskPoller(TimeSpan.FromSeconds(PollInterval), port),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        
                        break;
                    }
                }
                if (!found)
                {
                    Debug.LogWarning($"Could not find MIDI port {_portName} from {count} available");
                }
                else
                {
                    Debug.Log($"Found MIDI port {_portName}");
                }
            });
        }
        
        protected virtual void Awake()
        {
            _probe = new RtMIDIInputProbe();
            Scan();
        }

        protected virtual void OnDestroy()
        {
            _probe?.Dispose();
            _probe = null;
            
            _poller?.Dispose();
            _poller = null;
            
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}