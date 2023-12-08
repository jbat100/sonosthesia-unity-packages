using System;
using System.Text;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.RtMIDI
{
    public class RtMIDIInputStream : RawMIDIInputStream
    {
        [SerializeField] private string _port = "IAC Driver Unity";

        [SerializeField] private float _retryInterval = 1;

        private RtMIDIInputProbe _probe;
        private RtMIDIInputPort _inputPort;
        private IDisposable _subscription;

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
                    if (portName == _port)
                    {
                        found = true;
                        _subscription?.Dispose();
                        _inputPort?.Dispose();
                        _inputPort = new RtMIDIInputPort(i, portName);
                        break;
                    }
                }
                if (!found)
                {
                    Debug.LogWarning($"Could not find MIDI port {_port} from {count} available");
                }
                else
                {
                    Debug.Log($"Found MIDI port {_port}");
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
            _inputPort?.ProcessMessageQueue(this);
        }
        
        protected virtual void OnDestroy()
        {
            _probe?.Dispose();
            _probe = null;
            
            _inputPort?.Dispose();
            _inputPort = null;
            
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}