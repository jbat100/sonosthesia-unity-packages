using Sonosthesia.Utils;
using UnityEngine;

namespace Sonosthesia.FMOD
{
    public abstract class Volume : MonoBehaviour
    {
        [SerializeField] private string _path = ""; 
        
        [SerializeField] [Range(-80f, 10f)] private float _volume;

        protected void Start() => SetupPath(_path);

        protected virtual void Update() => ApplyLinearVolume(_volume.DecibelToLinear());

        protected abstract void SetupPath(string path);
        
        protected abstract void ApplyLinearVolume(float volume);
    }
}