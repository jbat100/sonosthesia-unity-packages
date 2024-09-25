using UnityEngine;

namespace Sonosthesia.Audio
{
    [CreateAssetMenu(
        fileName = "XAAConfiguration", 
        menuName = "Sonosthesia/Analysis/XAAConfiguration")]
    public class XAAConfiguration : ScriptableObject
    {
        [SerializeField] private PeakAnalysisFilterSettings _mainPeakFilter;
        [SerializeField] private PeakAnalysisFilterSettings _lowPeakFilter;
        [SerializeField] private PeakAnalysisFilterSettings _midPeakFilter;
        [SerializeField] private PeakAnalysisFilterSettings _highPeakFilter;

        [SerializeField] private AnalysisDBPreprocessor _mainPreprocessor;
        [SerializeField] private AnalysisDBPreprocessor _lowPreprocessor;
        [SerializeField] private AnalysisDBPreprocessor _midPreprocessor;
        [SerializeField] private AnalysisDBPreprocessor _highPreprocessor;

        public bool Check(PeakAnalysis analysis)
        {
            PeakAnalysisFilterSettings filter = analysis.channel switch
            {
                0 => _mainPeakFilter,
                1 => _lowPeakFilter,
                2 => _midPeakFilter,
                3 => _highPeakFilter,
                _ => null
            };
            return filter?.Check(analysis) ?? true;
        }

        public PeakAnalysis Process(PeakAnalysis analysis)
        {
            AnalysisDBPreprocessor preprocessor = analysis.channel switch
            {
                0 => _mainPreprocessor,
                1 => _lowPreprocessor,
                2 => _midPreprocessor,
                3 => _highPreprocessor,
                _ => null
            };

            if (preprocessor == null)
            {
                return analysis;
            }

            analysis.magnitude = preprocessor.Process(analysis.magnitude);
            return analysis;
        }

        public ContinuousAnalysis Process(ContinuousAnalysis analysis)
        {
            analysis.rms = _mainPreprocessor.Process(analysis.rms);
            analysis.lows = _mainPreprocessor.Process(analysis.lows);
            analysis.mids = _midPreprocessor.Process(analysis.mids);
            analysis.highs = _highPreprocessor.Process(analysis.highs);
            return analysis;
        }
    }
}