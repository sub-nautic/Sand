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

        private HandSide handSide;
        private Interactable currentInteractable;

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
            //show line renderer if can do something
            currentInteractable = RaycastInteractable();

            if (currentInteractable != null)
            {
                currentInteractable.HoverStart(this);
            }
            else
            {
                currentInteractable.HoverEnd(this);
            }
        }

        public void StartRaycastInteraction()
        {
            if (currentInteractable != null)
            {
                currentInteractable.InteractionStart(this);
            }
        }

        public void EndRaycastInteraction()
        {
            currentInteractable.InteractionEnd(this);
        }

        private Interactable RaycastInteractable()
        {
            if (Physics.Raycast(Transform.position, Transform.forward, out RaycastHit hit, maxRayDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                IInteractable interactable = hit.transform.GetComponentInParent<Interactable>();
                // float distance = Vector3.Distance(Transform.position, hit.point);
                return interactable?.AllowRayInteraction == true ? interactable as Interactable : null;
            }

            return null;
        }
    }
}
