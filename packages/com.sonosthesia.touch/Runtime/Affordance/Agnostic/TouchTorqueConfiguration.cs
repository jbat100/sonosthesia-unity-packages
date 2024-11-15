using UnityEngine;

namespace Sonosthesia.Touch
{
    [CreateAssetMenu(fileName = "TouchTorqueConfiguration", menuName = "Sonosthesia/Touch/TouchTorqueConfiguration")]
    public class TouchTorqueConfiguration : ScriptableObject
    {
        [SerializeField] private bool _track;
        public bool Track => _track;

        [SerializeField] private ForceMode _forceMode;
        public ForceMode ForceMode => _forceMode;

        [SerializeField] private bool _relative;
        public bool Relative => _relative;
        
        [SerializeField] private VectorTouchExtractorSettings _torque;
        public VectorTouchExtractorSettings Torque => _torque;
    }
}