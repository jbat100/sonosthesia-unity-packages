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
            IntervalThread
        }
        
        [SerializeField] private string _portName = "IAC Driver Unity";

        [SerializeField] private float _retryInterval = 1;

        [SerializeField] private PollMode _pollMode;
        
        [SerializeField] private float _pollInterval;

        private RtMIDIInputProbe _probe;
        private RtMIDIInputPort _port;
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
            
            public UpdatePoller(float interval, RtMIDIInputPort port) : base(port)
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
                    catch (OperationCanceledException)
                    {
                        // Handle the cancellation
                        break;
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
                            _port.ProcessMessageQueue();
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
                        _port?.Dispose();
                        _port = new RtMIDIInputPort(i, portName);
                        _port.NoteOnObservable.Subscribe(BroadcastNoteOn);
                        _port.NoteOffObservable.Subscribe(BroadcastNoteOff);
                        _port.ControlObservable.Subscribe(BroadcastControl);
                        _port.ChannelAftertouchObservable.Subscribe(BroadcastChannelAftertouch);
                        _port.PolyphonicAftertouchObservable.Subscribe(BroadcastPolyphonicAftertouch);
                        _port.PitchBendObservable.Subscribe(BroadcastPitchBend);
                        _port.SongPositionPointerObservable.Subscribe(BroadcastPositionPointer);
                        _port.SyncObservable.Subscribe(BroadcastSync);
                        _port.ClockObservable.Subscribe(BroadcastClock);
                        break;
                    }
                }
                if (!found)
                {
                    Debug.LogWarning($"Could not find MIDI port {_portName} from {count} available {_port}");
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

        protected virtual void Update()
        {
            //Debug.Log($"{nameof(MIDIInput)} port count {_probe.PortCount}");
            _port?.ProcessMessageQueue();
        }

        protected virtual void OnDestroy()
        {
            _probe?.Dispose();
            _port?.Dispose();
            _subscription?.Dispose();
        }
    }
}