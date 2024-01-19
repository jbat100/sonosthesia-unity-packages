using System;
using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TriggerStreamKiller : MonoBehaviour
    {
        private readonly Dictionary<Guid, TriggerSourceEvent> _kill = new ();

        protected void Kill(TriggerStream streams)
        {
            // make a copy as ending streams will cause the SourceStreamNode.Values to change during iteration
            _kill.Clear();
            _kill.Import(streams.SourceStreamNode.Values);
            foreach (KeyValuePair<Guid, TriggerSourceEvent> pair in _kill)
            {
                pair.Value.EndStream();
            }
        }
    }
}