using UnityEngine;
using Sonosthesia.Signal;
using Sonosthesia.Utils;

namespace Sonosthesia.Spawn
{
    [RequireComponent(typeof(BehaviourSelector))]
    public class BehaviourFloatAdaptor : MapAdaptor<BehaviourPayload, float>
    {
        private BehaviourSelector _selector;

        protected void Awake()
        {
            _selector = GetComponent<BehaviourSelector>();
        }
        
        protected override float Map(BehaviourPayload source)
        {
            return _selector != null ? _selector.Select(source) : 0f;
        }
    }
}