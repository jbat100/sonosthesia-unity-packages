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
        [SerializeField] private XAARelay _all;
        [SerializeField] private XAARelay _bass;
        [SerializeField] private XAARelay _drums;
        [SerializeField] private XAARelay _melody;
        [SerializeField] private XAARelay _instruments;
        
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
        
        private XAARelay CreateChildExtendedAudioAnalysisRelay(string childName)
        {
            XAARelay relay = CreateInstance<XAARelay>();
            relay.name = childName;
            AssetDatabase.AddObjectToAsset(relay, this);
            return relay;
        }
#endif
        
    }
}