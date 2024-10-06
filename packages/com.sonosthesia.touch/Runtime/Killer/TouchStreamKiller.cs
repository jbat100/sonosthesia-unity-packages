using System;
using System.Collections.Generic;
using UnityEngine;
using Sonosthesia.Utils;

namespace Sonosthesia.Touch
{
    public class TouchStreamKiller : MonoBehaviour
    {
        private readonly Dictionary<Guid, TouchEvent> _kill = new ();

        protected void Kill(TouchEventStreamContainer container)
        {
            // make a copy as ending streams will cause the SourceStreamNode.Values to change during iteration
            _kill.Clear();
            _kill.Import(container.StreamNode.Values);
            foreach (KeyValuePair<Guid, TouchEvent> pair in _kill)
            {
                pair.Value.EndStream();
            }
        }
    }
}