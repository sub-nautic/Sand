using System;
using System.Collections;
using System.Collections.Generic;
using Playground.Player.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Playground.Player.Interaction
{
    public class RaycastInteractor : IInteractor
    {
        public Transform Transform { get; private set; }
        public HandSide HandSide => HandSide;
        public float InteractionAmount { get; private set; }
        public bool IsHovering => currentHoverInteractableData != null;
        public bool IsInteracting => currentInteractableData != null;
        public InteractableData CurrentHoverInteractableData => currentHoverInteractableData;
        // public Interactable CurrentInteractable { get; private set; }
        // public InteractableData CurrentHoverInteractableData { get; private set; }

        private HandSide handSide;
        private InteractableData currentHoverInteractableData;
        private InteractableData currentInteractableData;

        private readonly LayerMask layerMask;
        private readonly float maxRayDistance;
        private readonly bool canHoverInteractables;

        public RaycastInteractor(Transform interactorTransform, LayerMask layerMask, float maxRayDistance, bool canHoverInteractables, HandSide handSide)
        {
            this.Transform = interactorTransform;
            this.layerMask = layerMask;
            this.maxRayDistance = maxRayDistance;
            this.canHoverInteractables = canHoverInteractables;
            this.handSide = handSide;
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
            // if (currentHoverInteractableData != null)
            // {
            //     currentHoverInteractableData.Interactable.InteractionStart(this);
            //     IsInteracting = true;
            // }

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
                IInteractable interactable = hit.transform.GetComponentInParent<Interactable>();
                float hitDistance = Vector3.Distance(Transform.position, hit.point);
                // Debug.Log($"[RaycastInteractor] hit: {hit.collider.name}");
                return interactable?.AllowRayInteraction == true ? new InteractableData(interactable, hitDistance, hit.point) : null;
            }

            return null;
        }
    }
}
