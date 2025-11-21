using System;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Channel
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(AbstractChannel), true)]
    public class AbstractChannelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AbstractChannel channel = (AbstractChannel)target;
            if(GUILayout.Button("Debug State"))
            {
                Debug.Log($"{channel.GetType().Name} has {channel.Ids.Count} ongoing streams");
            }
        }
    }
#endif
    
    // allows observers who do not need specific types but are just interested in stream counts / ids

    public abstract class AbstractChannel : MonoBehaviour, IGuidReactiveCollection
    {
        public abstract IReadOnlyReactiveCollection<Guid> Ids { get; }
    }
}