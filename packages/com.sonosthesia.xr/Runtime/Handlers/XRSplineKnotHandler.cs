using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.XR
{
    public class XRSplineKnotHandler : MonoBehaviour
    {
        [SerializeField] private XRGrabInteractable _interactable;
        
        [SerializeField] private SplineContainer _container;

        [SerializeField] private int _splineIndex;

        [SerializeField] private int _knotIndex;

        [SerializeField] private Vector3 _offset;

        protected Vector3 SplineKnotWorldPosition
        {
            get
            {
                Spline spline = _container[_splineIndex];
                BezierKnot knot = spline[_knotIndex];
                return _container.transform.TransformPoint(knot.Position);
            }
            set
            {
                Spline spline = _container[_splineIndex];
                BezierKnot knot = spline[_knotIndex];
                Vector3 position = _container.transform.InverseTransformPoint(value);
                knot.Position = value;
                spline[_knotIndex] = knot;
            }
        }

        protected Vector3 InteractableWorldPosition
        {
            get => _interactable.transform.position;
            set => _interactable.transform.position = value;
        }
        

        protected virtual void Update()
        {
            if (_interactable.isSelected)
            {
                SplineKnotWorldPosition = InteractableWorldPosition - _offset;
            }
            else
            {
                InteractableWorldPosition = SplineKnotWorldPosition + _offset;
            }
        }

    }
}