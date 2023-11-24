using Sonosthesia.AdaptiveMIDI;
using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.Pack
{
    public class PackedMIDIRawOutput : MIDIOutput
    {
        public override void BroadcastNoteOn(MIDINote note)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastNoteOff(MIDINote note)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastChannelControl(MIDIControl control)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastPolyphonicAftertouch(MIDIPolyphonicAftertouch aftertouch)
        {
            throw new System.NotImplementedException();
        }

        public override void BrodcatstChannelAftertouch(MIDIChannelAftertouch aftertouch)
        {
            throw new System.NotImplementedException();
        }

        public override void BroadcastPitchBend(MIDIPitchBend pitchBend)
        {
            throw new System.NotImplementedException();
        }
    }
}