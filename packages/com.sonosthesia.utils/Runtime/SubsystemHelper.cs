using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class SubsystemHelper
    {
        private static class SubsystemReuse<T> where T : ISubsystem
        {
            private static readonly List<T> _subsystemsReuse = new ();
            
            // note that XRHandTrackingEvent does work on every update to ensure that we are using the latest running
            // subsystem so maybe keeping a reference is not such a great idea
            
            public static T TryGetSubsystem()
            {
                SubsystemManager.GetSubsystems(_subsystemsReuse);
                return _subsystemsReuse.Count > 0 ? _subsystemsReuse[0] : default;
            }
        }

        public static T Get<T>() where T : ISubsystem
        {
            return SubsystemReuse<T>.TryGetSubsystem();
        }
    }
}