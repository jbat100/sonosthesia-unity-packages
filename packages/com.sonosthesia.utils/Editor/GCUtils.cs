using UnityEditor;
using UnityEngine;

namespace Sonosthesia.Utils.Editor
{
    // TODO: consider separate Profiling package
    
    public static class GarbageCollectorMenuItem
    {
        [MenuItem("Tools/Force Garbage Collection")]
        public static void ForceGarbageCollection()
        {
            Debug.Log("Forcing Garbage Collection...");
        
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        
            Debug.Log("Garbage Collection completed.");
        
            long totalMemory = System.GC.GetTotalMemory(false);
            Debug.Log($"Total memory after GC: {FormatBytes(totalMemory)}");
        }

        private static string FormatBytes(long bytes)
        {
            double processed = bytes;
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (processed >= 1024 && order < sizes.Length - 1)
            {
                order++;
                processed = processed / 1024;
            }
            return $"{processed:0.##} {sizes[order]}";
        }
    }
}