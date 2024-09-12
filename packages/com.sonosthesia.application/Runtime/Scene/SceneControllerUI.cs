using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Sonosthesia.Application
{
    public class SceneControllerUI : MonoBehaviour
    {
        [SerializeField] private SceneControllerSlot _sceneControllerSlot;

        [Header("State")] 
        
        [SerializeField] private Text _infoText;

        [Header("Controls")] 
        
        [SerializeField] private Slider _firstSlider;
        [SerializeField] private Slider _secondSlider;
        
        [Header("Toggles")]
        
        [SerializeField] private Toggle _firstToggle;
        [SerializeField] private Toggle _secondToggle;
        [SerializeField] private Toggle _thirdToggle;

        private IDisposable _subscription;

        protected virtual void Start()
        {
            _firstToggle.onValueChanged.AddListener(_ => Apply());
            _secondToggle.onValueChanged.AddListener(_ => Apply());
            _thirdToggle.onValueChanged.AddListener(_ => Apply());
            
            _firstSlider.onValueChanged.AddListener(_ => Apply());
            _secondSlider.onValueChanged.AddListener(_ => Apply());
        }
        
        protected virtual void OnEnable()
        {
            _subscription = _sceneControllerSlot.Observable.Subscribe(_ => Sync());
        }

        protected virtual void OnDisable()
        {
            _subscription?.Dispose();
        }

        protected virtual void Update()
        {
            SceneController controller = _sceneControllerSlot ? _sceneControllerSlot.Value : null;
            _infoText.text = controller ? controller.Info : "";
        }

        private void Sync()
        {
            List<int> indices = new List<int>();

            SceneController controller = _sceneControllerSlot ? _sceneControllerSlot.Value : null;
            if (controller)
            {
                indices.AddRange(controller.Indices);
            }

            _firstToggle.isOn = indices.Contains(0);
            _secondToggle.isOn = indices.Contains(1);
            _thirdToggle.isOn = indices.Contains(2);

            if (controller)
            {
                _firstSlider.value = controller.GetControl(0);
                _secondSlider.value = controller.GetControl(1);
            }
        }

        private void Apply()
        {
            List<int> indices = new List<int>();
            
            SceneController controller = _sceneControllerSlot ? _sceneControllerSlot.Value : null;
            if (!controller)
            {
                return;
            }

            if (_firstToggle.isOn)
            {
                indices.Add(0);
            }
            if (_secondToggle.isOn)
            {
                indices.Add(1);
            }
            if (_thirdToggle.isOn)
            {
                indices.Add(2);
            }

            controller.Indices = indices;
            
            controller.SetControl(0, _firstSlider.value);
            controller.SetControl(1, _secondSlider.value);
        }
        
    }
    
}

