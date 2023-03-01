using System.Collections.Generic;

namespace Sonosthesia.RtMIDI
{
    //
    // MIDI device driver class that manages all MIDI ports (interfaces) found
    // in the system.
    //
    internal sealed class RtMIDIDriver : System.IDisposable
    {
        #region Internal objects and methods

        private RtMIDIProbe _probe;
        private readonly List<RtMIDIPort> _ports = new List<RtMIDIPort>();

        private void ScanPorts()
        {
            for (var i = 0; i < _probe.PortCount; i++)
                _ports.Add(new RtMIDIPort(i, _probe.GetPortName(i)));
        }

        private void DisposePorts()
        {
            foreach (var p in _ports) p.Dispose();
            _ports.Clear();
        }

        #endregion

        #region Public methods

        public void Update()
        {
            if (_probe == null) _probe = new RtMIDIProbe();

            // Rescan the ports if the count of the ports doesn't match.
            if (_ports.Count != _probe.PortCount)
            {
                DisposePorts();
                ScanPorts();
            }

            // Process MIDI message queues in the port objects.
            foreach (var p in _ports) p.ProcessMessageQueue();
        }

        public void Dispose()
        {
            DisposePorts();

            _probe?.Dispose();
            _probe = null;
        }

        #endregion
    }
}
