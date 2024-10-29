using UnityEngine;

namespace Sonosthesia.Mapping
{
    public abstract class AbstractMapperConnection : MonoBehaviour
    {
#if UNITY_EDITOR        
        public abstract void AutofillSlots(bool recursive);
        
        public abstract void DeleteAllSlots();
#endif
    }
}