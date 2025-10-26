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

    public class AbstractChannel : MonoBehaviour, IGuidReactiveCollection
    {
        private readonly ReactiveCollection<Guid> _streamIds = new();
        public IReadOnlyReactiveCollection<Guid> Ids => _streamIds;

        protected void Register(Guid identifier)
        {
            if (!_streamIds.Contains(identifier))
            {
                _streamIds.Add(identifier);   
            }
        }

        protected void Unregister(Guid identifier)
        {
            _streamIds.Remove(identifier);
        }
    }
}