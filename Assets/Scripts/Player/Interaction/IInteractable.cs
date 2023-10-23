using System;
using UnityEngine;

namespace Playground.Player.Interaction
{
    public interface IInteractable
    {
        Transform Transform { get; set; }
        bool AllowRayInteraction { get; set; }

        // Event triggered when a new interactor starts hovering over this interactable.
        event Action<IInteractor> HoverStartEvent;
        // Event triggered when an interactor stops hovering over this interactable.
        event Action<IInteractor> HoverEndEvent;

        // Event triggered when an interactor's hand presses this interactable.
        event Action<IInteractor> InteractionStartEvent;
        // Event triggered when an interactor's hand releases the interact with this interactable.
        event Action<IInteractor> InteractionEndEvent;

        // Called when a new interactor starts hovering over this interactable.
        void HoverStart(IInteractor interactor);
        // Called when an interactor stops hovering over this interactable.
        void HoverEnd(IInteractor interactor);

        // Called when an interactor's hand presses this interactable.
        void InteractionStart(IInteractor interactor);
        // Called when an interactor's hand releases this interactable.
        void InteractionEnd(IInteractor interactor);
    }
}
