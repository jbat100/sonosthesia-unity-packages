namespace Sonosthesia.Utils
{
    // note : State and Intent have the same content but are different types in order to avoid confusion
    
    public readonly struct Intent
    {
        public static readonly Intent Empty = new Intent(null, null);
        
        public readonly string Key;
        public readonly object Payload;

        public Intent(string key, object payload)
        {
            Key = key;
            Payload = payload;
        }
        
        public Intent(string key)
        {
            Key = key;
            Payload = null;
        }

        public override string ToString()
        {
            return $"<{nameof(Intent)}:{Key}>";
        }
    }
}