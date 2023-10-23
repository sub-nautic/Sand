using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playground.Player.Interaction
{
    public interface IInteractable
    {
        Transform Transform { get; set; }
        bool AllowRayInteraction { get; set; }

        /// <summary>New interactor started hovering over this interactable</summary>
        event Action<IInteractor> HoverStartEvent;
        /// <summary>Interactor stopped hovering over this interactable</summary>
        event Action<IInteractor> HoverEndEvent;

        /// <summary>Hand pressed interact button, while hovering/grabbing this interactable</summary>
        event Action<IInteractor> InteractionStartEvent;
        /// <summary>Hand released interact button, while hovering/grabbing this interactable</summary>
        event Action<IInteractor> InteractionEndEvent;

        /// <summary>Interactor started hovering over this interactable</summary>
        void HoverStart(IInteractor interactor);
        /// <summary>Interactor stopped hovering over this interactable</summary>
        void HoverEnd(IInteractor interactor);

        /// <summary>Interactor pressed interact button, while hovering/grabbing this interactable</summary>
        void InteractionStart(IInteractor interactor);
        /// <summary>Interactor released interact button, while hovering/grabbing this interactable</summary>
        void InteractionEnd(IInteractor interactor);
    }
}
