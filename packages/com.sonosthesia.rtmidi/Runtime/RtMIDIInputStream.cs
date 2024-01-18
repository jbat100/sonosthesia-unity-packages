using System;
using System.Text;
using UniRx;
using UnityEngine;
using Sonosthesia.AdaptiveMIDI;

namespace Sonosthesia.RtMIDI
{
    public class RtMIDIInputStream : RawMIDIInputStream
    {
        [SerializeField] private string _portName = "IAC Driver Unity";

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
                if (_probe.TryGet(_portName, out string actualPortName, out int actualPortNumber))
                {
                    _subscription?.Dispose();
                    _inputPort?.Dispose();
                    _inputPort = new RtMIDIInputPort(actualPortNumber, actualPortName);
                    Debug.Log($"Found MIDI input port {actualPortName} (number {actualPortNumber}) for requested : {_portName}");
                }
                else
                {
                    Debug.LogWarning($"Could not find MIDI input port for requested {_portName} from {_probe.PortCount} available");
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