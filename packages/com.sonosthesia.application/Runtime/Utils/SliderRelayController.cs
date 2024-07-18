using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sonosthesia.Application
{
    // allows a slider to control a process through relays (across scenes)
    
    [RequireComponent(typeof(Slider))]
    public class SliderRelayController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private IntentSignalRelay _intent;

        [SerializeField] private FloatSignalRelay _value;
        
        [SerializeField] private FloatSignalRelay _duration;

        private Slider _slider;
        private bool _isDown;
        private readonly CompositeDisposable _subscriptions = new();

        protected virtual void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        protected virtual void OnEnable()
        {
            _isDown = false;
            _subscriptions.Clear();

            if (_value)
            {
                _subscriptions.Add(_value.Observable.DistinctUntilChanged().Subscribe(v =>
                {
                    if (!_isDown && _slider)
                    {
                        _slider.value = v;
                    }
                }));
            }

            if (_duration)
            {
                _subscriptions.Add(_duration.Observable.DistinctUntilChanged().Subscribe(d =>
                {
                    if (_slider)
                    {
                        _slider.maxValue = d;
                    }
                }));
            }
        }

        protected virtual void OnDisable()
        {
            _isDown = false;
            _subscriptions.Clear();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDown = false;
            
            if (!_intent || !_slider)
            {
                return;
            }
            
            _intent.Broadcast(new Intent(ApplicationIntentKeys.TIME, _slider.value));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDown = true;
        }
    }
}