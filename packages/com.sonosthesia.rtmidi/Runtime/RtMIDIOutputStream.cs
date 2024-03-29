using System;
using System.Text;
using Sonosthesia.AdaptiveMIDI;
using UniRx;
using UnityEngine;

namespace Sonosthesia.RtMIDI
{
    public class RtMIDIOutputStream : RawMIDIOutputStream
    {
        [SerializeField] private string _portName = "IAC Driver Unity";

        [SerializeField] private float _retryInterval = 1;

        private RtMIDIOutputProbe _probe;
        private RtMIDIOutputPort _port;
        private IDisposable _subscription;
        
        private string Description()
        {
            StringBuilder builder = new StringBuilder("Rt MIDI output scan : ");
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
                    _port?.Dispose();
                    _port = new RtMIDIOutputPort(actualPortNumber, actualPortName);
                    Debug.Log($"Found MIDI output port {actualPortName} (number {actualPortNumber}) for requested : {_portName}");
                }
                else
                {
                    Debug.LogWarning($"Could not find MIDI output port for requested {_portName} from {_probe.PortCount} available");
                }
            });
        }
        
        
        protected virtual void Awake()
        {
            _probe = new RtMIDIOutputProbe();
            Scan();
        }

        protected virtual void OnDestroy()
        {
            _probe?.Dispose();
            _probe = null;
            
            _port?.Dispose();
            _port = null;
            
            _subscription?.Dispose();
            _subscription = null;
        }

        public override void Broadcast(byte data0)
        {
            _port?.Broadcast(data0);
        }

        public override void Broadcast(byte data0, byte data1)
        {
            _port?.Broadcast(data0, data1);
        }

        public override void Broadcast(byte data0, byte data1, byte data2)
        {
            _port?.Broadcast(data0, data1, data2);
        }
    }
}