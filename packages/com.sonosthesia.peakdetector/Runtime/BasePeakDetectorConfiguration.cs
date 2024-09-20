using UnityEngine;

namespace Sonosthesia.PeakDetector
{
    public abstract class BasePeakDetectorConfiguration : ScriptableObject
    {
        public abstract PeakDetectorSettings Settings { get; }
    }
}