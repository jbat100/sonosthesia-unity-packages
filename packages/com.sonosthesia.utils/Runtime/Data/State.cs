namespace Sonosthesia.Utils
{
    // note : State and Intent have the same content but are different types in order to avoid confusion

    public readonly struct State
    {
        public readonly string Key;
        public readonly object Payload;

        public State(string key, object payload)
        {
            Key = key;
            Payload = payload;
        }
        
        public State(string key)
        {
            Key = key;
            Payload = null;
        }

        public override string ToString()
        {
            return $"<{nameof(State)}:{Key}>";
        }
    }
}
