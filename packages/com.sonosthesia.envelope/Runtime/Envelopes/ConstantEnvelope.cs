namespace Sonosthesia.Envelope
{
    public class ConstantEnvelope : IEnvelope
    {
        public float Duration => _duration;
        public float InitialValue => _value;
        public float FinalValue => _value;

        private float _duration;
        private float _value;

        public ConstantEnvelope(float value, float duration)
        {
            _duration = duration;
            _value = value;
        }

        public float Evaluate(float time) => _value;
    }
}