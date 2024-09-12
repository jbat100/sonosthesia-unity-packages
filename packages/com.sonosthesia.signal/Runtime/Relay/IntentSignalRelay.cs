using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Signal
{
    [CreateAssetMenu(fileName = "IntentSignalRelay", menuName = "Sonosthesia/Relays/IntentSignalRelay")]
    public class IntentSignalRelay : StatelessSignalRelay<Intent>
    {
        public void Broadcast(string key)
        {
            Broadcast(new Intent(key, null));
        }
    }
}