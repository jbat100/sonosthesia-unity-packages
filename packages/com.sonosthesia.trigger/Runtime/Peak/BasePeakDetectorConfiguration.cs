using UnityEngine;

namespace Sonosthesia.Trigger
{
    public abstract class BasePeakDetectorConfiguration : ScriptableObject
    {
        public abstract PeakDetectorSettings Settings { get; }
    }
}