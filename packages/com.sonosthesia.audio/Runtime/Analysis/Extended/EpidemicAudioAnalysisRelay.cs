using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sonosthesia.Audio
{
    // TODO : this is not functional, the objective is to ease the creation of multi part analysis relays
    // [CreateAssetMenu(fileName = "EpidemicAudioAnalysisRelay", menuName = "Sonosthesia/Relays/EpidemicAudioAnalysisRelay")]
    public class EpidemicAudioAnalysisRelay : ScriptableObject
    {
        [SerializeField] private ExtendedAudioAnalysisRelay _all;
        [SerializeField] private ExtendedAudioAnalysisRelay _bass;
        [SerializeField] private ExtendedAudioAnalysisRelay _drums;
        [SerializeField] private ExtendedAudioAnalysisRelay _melody;
        [SerializeField] private ExtendedAudioAnalysisRelay _instruments;
        
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            CreateRelays();
#endif
        }
        
#if UNITY_EDITOR
        [ContextMenu("Create Relays")]
        private void CreateRelays()
        {
            _all = CreateChildExtendedAudioAnalysisRelay("All");
            _bass = CreateChildExtendedAudioAnalysisRelay("Bass");
            _drums = CreateChildExtendedAudioAnalysisRelay("Drums");
            _melody = CreateChildExtendedAudioAnalysisRelay("Melody");
            _instruments = CreateChildExtendedAudioAnalysisRelay("Instruments");
            
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }
        
        private ExtendedAudioAnalysisRelay CreateChildExtendedAudioAnalysisRelay(string childName)
        {
            ExtendedAudioAnalysisRelay relay = CreateInstance<ExtendedAudioAnalysisRelay>();
            relay.name = childName;
            AssetDatabase.AddObjectToAsset(relay, this);
            return relay;
        }
#endif
        
    }
}