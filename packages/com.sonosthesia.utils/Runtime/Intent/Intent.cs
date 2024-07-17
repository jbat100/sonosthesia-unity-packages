namespace Sonosthesia.Utils
{
    public readonly struct Intent
    {
        public readonly string Key;
        public readonly object Payload;

        public Intent(string key, object payload)
        {
            Key = key;
            Payload = payload;
        }
    }
}