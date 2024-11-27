using UnityEngine;

namespace Sonosthesia.Utils
{
    public interface ILogSwitch
    {
        bool Log { get; }
    }

    public static class ConditionalLog
    {
        public static int Level = 2;
        
        public static void LogVerbose(this ILogSwitch logSwitch, string message)
        {
            if (logSwitch.Log && Level >= 2)
            {
                Debug.Log(message);   
            }
        }
        
        public static void LogWarning(this ILogSwitch logSwitch, string message)
        {
            if (logSwitch.Log && Level >= 1)
            {
                Debug.LogWarning(message);   
            }
        }
        
        public static void LogError(this ILogSwitch logSwitch, string message)
        {
            if (Level >= 0)
            {
                Debug.LogError(message);   
            }
        }
    }
}