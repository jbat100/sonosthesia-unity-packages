using System;
using System.Collections.Generic;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TouchEventStreamContainer : StreamContainer<TouchEvent>
    {
        public void KillStream(Guid id)
        {
            if (StreamNode.Values.TryGetValue(id, out TouchEvent e))
            {
                e.TouchData.Source.KillStream(id);
            }
        }
        
        public void KillAllStreams()
        {
            List<KeyValuePair<Guid, TouchEvent>> all = new (StreamNode.Values);
            foreach (KeyValuePair<Guid, TouchEvent> pair in all)
            {
                pair.Value.TouchData.Source.KillStream(pair.Key);
            }
        }
    }
}