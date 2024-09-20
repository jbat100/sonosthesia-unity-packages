using UnityEngine;

namespace Sonosthesia.PeakDetector
{
    [CreateAssetMenu(fileName = "PeakDetector", menuName = "Sonosthesia/Peak/PeakDetector")]
    public class PeakDetectorConfiguration : BasePeakDetectorConfiguration
    {
        [SerializeField] private PeakDetectorSettings _settings;

        public override PeakDetectorSettings Settings => _settings;
    }
}