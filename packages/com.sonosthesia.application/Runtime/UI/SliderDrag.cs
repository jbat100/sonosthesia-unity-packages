using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(Slider))]
    public class SliderDrag : MonoBehaviour, IPointerUpHandler 
    {
        [Serializable]
        public class SliderEndEvent : UnityEvent<float> {}
        
        [SerializeField]
        private SliderEndEvent _onEnd = new ();

        private Slider _slider;

        public SliderEndEvent onEnd => _onEnd;

        public Slider Slider => _slider;

        protected void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public float value
        {
            get => _slider ? _slider.value : 0f;
            set
            {
                if (_slider)
                {
                    _slider.value = value;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _onEnd.Invoke(_slider.value);
        }
    }
}