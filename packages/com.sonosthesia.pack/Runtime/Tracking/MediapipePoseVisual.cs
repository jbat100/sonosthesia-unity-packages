using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Sonosthesia.Pack
{
    public class MediapipePoseVisual : MonoBehaviour
    {
        [SerializeField] private ContentReceiver<MediapipePose> _receiver;

        [SerializeField] private MediapipePoseVisualElement _prefab;

        private readonly List<MediapipePoseVisualElement> _instances = new ();
        private IDisposable _subscription;
        private MediapipePose _pose;

        
        protected virtual void Awake()
        {
            if (!_receiver)
            {
                _receiver = GetComponentInParent<ContentReceiver<MediapipePose>>();
            }
        }

        protected virtual void OnEnable()
        {
            CleanElements();
            _subscription?.Dispose();
            _subscription = _receiver.ContentObservable.ObserveOnMainThread().Subscribe(pose => _pose = pose);
            
            List<Func<Point>> _fetchers = new List<Func<Point>>
            {
                () => _pose?.Nose,
                () => _pose?.LeftEyeInner,
                () => _pose?.LeftEye,
                () => _pose?.LeftEyeOuter,
                () => _pose?.RightEyeInner,
                () => _pose?.RightEye,
                () => _pose?.RightEyeOuter,
                () => _pose?.LeftEar,
                () => _pose?.RightEar,
                () => _pose?.MouthLeft,
                () => _pose?.MouthRight,
                () => _pose?.LeftShoulder,
                () => _pose?.RightShoulder,
                () => _pose?.LeftElbow,
                () => _pose?.RightElbow,
                () => _pose?.LeftWrist,
                () => _pose?.RightWrist,
                () => _pose?.LeftPinky,
                () => _pose?.RightPinky,
                () => _pose?.LeftIndex,
                () => _pose?.RightIndex,
                () => _pose?.LeftThumb,
                () => _pose?.RightThumb,
                () => _pose?.LeftHip,
                () => _pose?.RightHip,
                () => _pose?.LeftKnee,
                () => _pose?.RightKnee,
                () => _pose?.LeftAnkle,
                () => _pose?.RightAnkle,
                () => _pose?.LeftHeel,
                () => _pose?.RightHeel,
                () => _pose?.LeftFootIndex,
                () => _pose?.RightFootIndex
            };
            
            _instances.AddRange(_fetchers.Select(fetchPoint =>
            {
                return CreateElement(fetchPoint, () => _pose?.Nose);
            }));
        }
        
        protected virtual void OnDisable()
        {
            CleanElements();
            _subscription?.Dispose();
        }

        private void CleanElements()
        {
            foreach (MediapipePoseVisualElement element in _instances)
            {
                Destroy(element.gameObject);
            }
            _instances.Clear();
        }
        
        private MediapipePoseVisualElement CreateElement(Func<Point> fetchPoint, Func<Point> fetchReference)
        {
            MediapipePoseVisualElement instance = Instantiate(_prefab);
            instance.FetchPoint = fetchPoint;
            instance.FetchReference = fetchReference;
            return instance;
        }
    }
}
