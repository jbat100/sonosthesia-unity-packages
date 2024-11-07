using System.Collections.Generic;
using UnityEngine;

namespace Sonosthesia.Utils
{
    public static class SubsystemHelper
    {
        private static class SubsystemReuse<T> where T : ISubsystem
        {
            private static readonly List<T> _subsystemsReuse = new ();
            
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