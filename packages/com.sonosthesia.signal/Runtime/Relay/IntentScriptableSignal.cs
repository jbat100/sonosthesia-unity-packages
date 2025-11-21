using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.Signal
{
    public static class IntentSignalExtension
    {
        public static void Broadcast(this ISignal<Intent> signal, string key) =>
            signal.Broadcast(new Intent(key, null));
    }
    
    [CreateAssetMenu(fileName = "IntentSignal", menuName = "Sonosthesia/Signals/IntentSignal")]
    public class IntentScriptableSignal : ScriptableSignal<Intent>
    {
        
    }
}