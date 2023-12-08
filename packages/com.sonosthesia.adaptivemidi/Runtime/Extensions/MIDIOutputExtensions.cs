using Sonosthesia.AdaptiveMIDI.Messages;

namespace Sonosthesia.AdaptiveMIDI.Extensions
{
    public static class MIDIOutputExtensions
    {
        public static void BroadcastNoteOn(this MIDIOutput broadcaster, 
            int channel, int note, int velocity)
        {
            broadcaster.Broadcast(new MIDINoteOn(channel, note, velocity));
        }
        
        public static void BroadcastNoteOff(this MIDIOutput broadcaster, 
            int channel, int note, int velocity)
        {
            broadcaster.Broadcast(new MIDINoteOff(channel, note, velocity));
        }
        
        public static void BroadcastControl(this MIDIOutput broadcaster,
            int channel, int number, int value)
        {
            broadcaster.Broadcast(new MIDIControl(channel, number, value));
        }
        
        public static void BroadcastChannelAftertouch(this MIDIOutput broadcaster,
            int channel, int value)
        {
            broadcaster.Broadcast(new MIDIChannelAftertouch(channel, value));
        }
        
        public static void BroadcastPolyphonicAftertouch(this MIDIOutput broadcaster,
            int channel, int note, int value)
        {
            broadcaster.Broadcast(new MIDIPolyphonicAftertouch(channel, note, value));
        }
        
        public static void BroadcastPitchBend(this MIDIOutput broadcaster,
            int channel, float semitones, float range = 48f)
        {
            broadcaster.Broadcast(new MIDIPitchBend(channel, semitones, range));
        }
    }
}