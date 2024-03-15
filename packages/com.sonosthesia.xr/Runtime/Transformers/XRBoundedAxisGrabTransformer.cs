using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using Sonosthesia.Utils;

namespace Sonosthesia.XR
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="XRGrabInteractable"/>
    [AddComponentMenu("XR/Transformers/XR Single Grab Free Transformer", 11)]
    public class XRBoundedAxisGrabTransformer : XRBaseGrabTransformer
    {
        [SerializeField] private Axes _displacementAxes;

        [SerializeField] private Transform _lowerBound;
        
        [SerializeField] private Transform _upperBound;

        private Pose _grabInteractablePose;
        private Pose _grabInteractorPose;
        private Vector3 _attachOffset;
        
        /// <inheritdoc />
        public override void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                {
                    UpdateTarget(grabInteractable, ref targetPose);

                    break;
                }
            }
        }

        public override void OnGrab(XRGrabInteractable grabInteractable)
        {
            if (grabInteractable.interactorsSelecting.Count > 0)
            {
                IXRSelectInteractor interactor = grabInteractable.interactorsSelecting[0];
                _grabInteractablePose = grabInteractable.transform.GetWorldPose();
                _grabInteractorPose = interactor.transform.GetWorldPose();
                _attachOffset = _grabInteractablePose.position - grabInteractable.GetAttachTransform(interactor).position;
            }
        }

        private void UpdateTarget(XRGrabInteractable grabInteractable, ref Pose targetPose)
        {
            IXRSelectInteractor interactor = grabInteractable.interactorsSelecting[0];
            
            Pose interactorPose = interactor.transform.GetWorldPose();
            Vector3 worldDrag = _grabInteractorPose.position - interactorPose.position;

            // calculate the drag in interactable space
            Vector3 interactableDrag = grabInteractable.transform.InverseTransformDirection(worldDrag);
            // apply axes filter while in interactable space
            Vector3 filteredInteractableDrag = interactableDrag.FilterAxes(_displacementAxes);
            // transform back to world
            Vector3 filteredWorldDrag = grabInteractable.transform.TransformDirection(filteredInteractableDrag);

            Vector3 targetPosition = _attachOffset + _grabInteractorPose.position + filteredWorldDrag;
            

            // Pose interactorAttachPose = interactor.GetAttachTransform(grabInteractable).GetWorldPose();
            // Pose thisTransformPose = grabInteractable.transform.GetWorldPose();
            // Transform thisAttachTransform = grabInteractable.GetAttachTransform(interactor);

            // Calculate offset of the grab interactable's position relative to its attach transform
            // var attachOffset = thisTransformPose.position - thisAttachTransform.position;
            // targetPose.position = attachOffset + interactorAttachPose.position;

        }

    }
}
