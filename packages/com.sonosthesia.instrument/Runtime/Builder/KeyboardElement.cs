using UnityEngine;

namespace Sonosthesia.Instrument
{
    public class KeyboardElement : MonoBehaviour, IMIDIPitchedElement, IIndexed
    {
        public int MIDINote { get; set; }
        
        public int Index { get; set; }

        [SerializeField] private Transform _scaleTarget;

        public Transform ScaleTarget
        {
            get
            {
                if (!_scaleTarget)
                {
                    return transform;
                }

                return _scaleTarget;
            }
        }

        private Renderer _renderer;

        public Renderer Renderer
        {
            get
            {
                if (_renderer)
                {
                    return _renderer;
                }

                _renderer = GetComponentInChildren<Renderer>();
                return _renderer;
            }
        }
        
        public Color Color
        {
            get => Renderer.material.color;
            set => Renderer.material.color = value;
        }
    }
}