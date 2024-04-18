using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sonosthesia.XR
{
    // looked into using the affordance system but it's long winded for this use case and is deprecated in version 3
    
    public class XRBoundedAxisGrabLineIndicator : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractable _interactable;
        
        [SerializeField] private XRBoundedAxisGrabTransformer _transformer;
        
        protected virtual void OnEnable()
        {
            _interactable.activated.AddListener(OnInteractableActivated);
            _interactable.activated.AddListener(OnInteractableDeactivated);
            
            _interactable.selectEntered.AddListener(OnInteractableSelectEntered);
            _interactable.selectExited.AddListener(OnInteractableSelectExited);
        }

        protected virtual void OnDisable()
        {
            _interactable.activated.RemoveListener(OnInteractableActivated);
            _interactable.activated.RemoveListener(OnInteractableDeactivated);
            
            _interactable.selectEntered.RemoveListener(OnInteractableSelectEntered);
            _interactable.selectExited.RemoveListener(OnInteractableSelectExited);
        }

        protected virtual void OnInteractableActivated(ActivateEventArgs args)
        {
            
        }
        
        protected virtual void OnInteractableDeactivated(ActivateEventArgs args)
        {
            
        }
        
        protected virtual void OnInteractableSelectEntered(SelectEnterEventArgs arg0)
        {
            
        }
        
        protected virtual void OnInteractableSelectExited(SelectExitEventArgs arg0)
        {
            
        }
    }
}
