namespace Sonosthesia.Envelope
{
    public class WarpedEnvelope : IEnvelope
    {
        private readonly IEnvelope _envelope;
        private readonly float _valueScale;
        private readonly float _timeScale;

        public WarpedEnvelope(IEnvelope envelope, float valueScale, float timeScale)
        {
            _envelope = envelope;
            _valueScale = valueScale;
            _timeScale = timeScale;
        }
        
        public float Duration => _envelope.Duration * _timeScale;
        
        public float InitialValue => _envelope.InitialValue * _valueScale;
        
        public float FinalValue => _envelope.FinalValue * _valueScale;
        
        public float Evaluate(float time) => _envelope.Evaluate(time / _timeScale) * _valueScale;
    }
}