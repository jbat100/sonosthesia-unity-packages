using System;
using System.Collections.Generic;
using Sonosthesia.Utils;
using Sonosthesia.Channel;

namespace Sonosthesia.Touch
{
    public class TouchEventChannel : Channel<TouchEvent>
    {
        private static readonly List<KeyValuePair<Guid, TouchEvent>> _valuesReuse = new();
        
        public void KillStream(Guid id)
        {
            if (Values.TryGetValue(id, out TouchEvent e))
            {
                e.touchData.Source.KillStream(id);
            }
        }
        
        public void KillAllStreams()
        {
            foreach (KeyValuePair<Guid, TouchEvent> pair in _valuesReuse.Import(Values))
            {
                pair.Value.touchData.Source.KillStream(pair.Key);
            }
        }
    }
}