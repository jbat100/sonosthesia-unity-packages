using Sonosthesia.Signal;
using Sonosthesia.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sonosthesia.Application
{
    [RequireComponent(typeof(Button))]
    public class ButtonEventIntents : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private IntentScriptableSignal _target;
        
        [Header("Intent keys")]
        
        [SerializeField] private string _pointerEnter;
        
        [SerializeField] private string _pointerClick;

        private Button _button;

        protected virtual void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Push(_pointerEnter);
        }

        private void OnButtonClicked()
        {
            Push(_pointerClick);
        }

        private void Push(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            
            if (!_target)
            {
                return;
            }
            
            _target.Broadcast(new Intent(key));
        }
    }
}