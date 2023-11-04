using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OverlordVR.Player.Interaction
{
    public class RaycastInteractor : IInteractor
    {
        public Transform Transform { get; private set; }
        public Vector3 HitPosition => currentInteractableData.HitPosition;
        public Vector3 HoverHitPosition => currentHoverInteractableData.HitPosition;
        public HandSide HandSide => HandSide;
        public float InteractionAmount { get; private set; }
        public bool IsHovering => currentHoverInteractableData != null;
        public bool IsInteracting => currentInteractableData != null;
        public InteractableData CurrentHoverInteractableData => currentHoverInteractableData;

        private InteractableData currentHoverInteractableData;
        private InteractableData currentInteractableData;

        private readonly LayerMask layerMask;
        private readonly float maxRayDistance;
        private readonly bool canHoverInteractables;

        public RaycastInteractor(Transform interactorTransform, LayerMask layerMask, float maxRayDistance, bool canHoverInteractables)
        {
            this.Transform = interactorTransform;
            this.layerMask = layerMask;
            this.maxRayDistance = maxRayDistance;
            this.canHoverInteractables = canHoverInteractables;
        }

        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public void UpdateRaycastInteractor(float interactionAmount)
        {
            InteractionAmount = interactionAmount;

            if (canHoverInteractables)
            {
                UpdateRaycastHover();
            }
        }

        private void UpdateRaycastHover()
        {
            InteractableData interactableData = RaycastInteractable();

            currentHoverInteractableData?.Interactable.HoverEnd(this);
            currentHoverInteractableData = interactableData;
            currentHoverInteractableData?.Interactable.HoverStart(this);
        }

        public void StartRaycastInteraction()
        {
            currentInteractableData?.Interactable.InteractionEnd(this);
            currentInteractableData = RaycastInteractable();
            currentInteractableData?.Interactable.InteractionStart(this);
        }

        public void EndRaycastInteraction()
        {
            currentInteractableData?.Interactable.InteractionEnd(this);
            currentInteractableData = null;
        }

        private InteractableData RaycastInteractable()
        {
            if (Physics.Raycast(Transform.position, Transform.forward, out RaycastHit hit, maxRayDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                // Get the interactable component from the hit object.
                IInteractable interactable = hit.transform.GetComponentInParent<Interactable>();

                // Calculate the distance between the interactor and the hit point.
                float hitDistance = Vector3.Distance(Transform.position, hit.point);

                // Check if ray interaction is allowed for the interactable and return interactable data.
                return interactable?.AllowRayInteraction == true ? new InteractableData(interactable, hitDistance, hit.point) : null;
            }

            // No valid interactable found.
            return null;
        }
    }
}
