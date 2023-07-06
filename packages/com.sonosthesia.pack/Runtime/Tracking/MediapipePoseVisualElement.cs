using System;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class MediapipePoseVisualElement : MonoBehaviour
    {
        [SerializeField] private Gradient _visibility;
        [SerializeField] private GameObject _visual;
        [SerializeField] private float _threshold = 0.9f;

        private Renderer _renderer;
        
        public Func<Point> FetchPoint { get; set; }
        
        public Func<Point> FetchReference { get; set; }

        protected virtual void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
        }
        
        protected virtual void Update()
        {
            Point point = FetchPoint?.Invoke();
            if (point != null && point.Visibility > _threshold)
            {
                if (!_visual.activeSelf)
                {
                    _visual.SetActive(true);
                }
                Vector3 referencePosition = Vector3.zero;
                Point reference = FetchReference?.Invoke();
                if (reference != null && reference.Visibility > _threshold)
                {
                    referencePosition = reference.ToVector3();
                }
                transform.localPosition = point.ToVector3() - referencePosition;
                //Color color = _visibility.Evaluate(point.Visibility);
                //_renderer.material.color = color;
            }
            else
            {
                if (_visual.activeSelf)
                {
                    _visual.SetActive(false);
                }
            }
        }
    }
}