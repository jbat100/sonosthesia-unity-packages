using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Sonosthesia.Utils
{
    [CreateAssetMenu(fileName = "FrameModerator", menuName = "Sonosthesia/Utils/FrameModerator")]
    public class FrameModerator : ScriptableObject
    {
        [SerializeField] private int _maxFrameSpan = 3;
        
        private readonly Dictionary<Guid, int> _participants = new ();

        private static readonly List<Guid> _reuseGuids = new();

        private int _slots = 0; 

        public void Register(Guid id)
        {
            _participants[id] = 0;
            Distribute();
        }

        public void Unregister(Guid id)
        {
            _participants.Remove(id);
            Distribute();
        }

        public bool Request(Guid id)
        {
            if (_participants.TryGetValue(id, out int slot))
            {
                return Time.frameCount % _slots == slot;
            }
            return false;
        }

        private void Distribute()
        {
            int slot = 0;
            _slots = math.min(_participants.Count, _maxFrameSpan);
            foreach (Guid id in _reuseGuids.Import(_participants.Keys))
            {
                _participants[id] = slot % _slots;
                slot++;
            }
        }
    }
}