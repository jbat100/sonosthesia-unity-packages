using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sonosthesia.FMOD
{
#if UNITY_EDITOR    
    [CustomEditor(typeof(FMODCoreTest))]
    public class FMODCoreTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            FMODCoreTest test = (FMODCoreTest)target;
            
            // Add a button and handle the click event
            if (GUILayout.Button("Channel Groups"))
            {
                test.ChannelGroups();
            }
        }
    }
#endif
    
    public class FMODCoreTest : MonoBehaviour
    {
        
        [SerializeField] private EventReference _fmodEventPath;
        
        private EventInstance eventInstance;
        private ChannelGroup[] channelGroups;
        
        protected virtual void Start()
        {
            // Create an instance of the event
            eventInstance = RuntimeManager.CreateInstance(_fmodEventPath);
            
            
            // Start the event
            eventInstance.start();
            
            
        }

        public void ChannelGroups()
        {
            
        }
        

        void OnDestroy()
        {
            // Stop and release the event instance when the object is destroyed
            eventInstance.stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }    
}


