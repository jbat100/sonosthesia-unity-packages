using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.XR
{
    public class XRPoseHandler : MonoBehaviour
    {
        [SerializeField] private XRGrabInteractable _interactable;

        [SerializeField] private Transform _target;

        [SerializeField] private Vector3 _offset;

        protected virtual void Update()
        {
            if (_interactable.isSelected)
            {
                _target.position = _interactable.transform.position;
                _target.rotation = _interactable.transform.rotation;
            }
            else
            {
                _interactable.transform.position = _target.position;
                _interactable.transform.rotation = _target.rotation;
            }
        }
    }
}