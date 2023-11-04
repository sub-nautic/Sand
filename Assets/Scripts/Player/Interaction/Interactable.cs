using System;
using UnityEngine;

namespace OverlordVR.Player.Interaction
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        [Tooltip("Determines if ray interaction is allowed for this object")]
        [SerializeField] private bool allowRayInteraction = true;

        public Transform Transform { get; set; }
        public bool AllowRayInteraction { get => allowRayInteraction; set => allowRayInteraction = value; }

        public event Action<IInteractor> HoverStartEvent;
        public event Action<IInteractor> HoverEndEvent;
        public event Action<IInteractor> InteractionStartEvent;
        public event Action<IInteractor> InteractionEndEvent;

        public void HoverStart(IInteractor interactor)
        {
            HoverStartEvent?.Invoke(interactor);
        }

        public void HoverEnd(IInteractor interactor)
        {
            HoverEndEvent?.Invoke(interactor);
        }

        public void InteractionStart(IInteractor interactor)
        {
            InteractionStartEvent?.Invoke(interactor);
        }

        public void InteractionEnd(IInteractor interactor)
        {
            InteractionEndEvent?.Invoke(interactor);
        }
    }
}