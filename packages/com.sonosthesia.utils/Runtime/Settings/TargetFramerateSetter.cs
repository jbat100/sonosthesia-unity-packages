using UnityEngine;

namespace Sonosthesia.Utils
{
    // TODO : switch to scriptable object, and switch based on platform
    
    public class TargetFramerateSetter : MonoBehaviour
    {
        [SerializeField] private int _targetFramerate = -1;

        protected void OnEnable()
        {
            Application.targetFrameRate = _targetFramerate;
        }

    }
}