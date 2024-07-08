using UnityEngine;

namespace Sonosthesia.Trigger
{
    [CreateAssetMenu(fileName = "PeakDetector", menuName = "Sonosthesia/Peak/PeakDetector")]
    public class PeakDetectorConfiguration : BasePeakDetectorConfiguration
    {
        [SerializeField] private PeakDetectorSettings _settings;

        public override PeakDetectorSettings Settings => _settings;
    }
}