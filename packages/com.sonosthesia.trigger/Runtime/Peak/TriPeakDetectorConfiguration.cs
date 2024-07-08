using UnityEngine;

namespace Sonosthesia.Trigger
{
    [CreateAssetMenu(fileName = "PeakDetector", menuName = "Sonosthesia/Peak/TriPeakDetector")]
    public class TriPeakDetectorConfiguration : ScriptableObject
    {
        [SerializeField] private PreprocessedPeakDetectorSettings _lows;
        public PreprocessedPeakDetectorSettings Lows => _lows;

        [SerializeField] private PreprocessedPeakDetectorSettings _mids;
        public PreprocessedPeakDetectorSettings Mids => _mids;
        
        [SerializeField] private PreprocessedPeakDetectorSettings _highs;
        public PreprocessedPeakDetectorSettings Highs => _highs;
    }
}