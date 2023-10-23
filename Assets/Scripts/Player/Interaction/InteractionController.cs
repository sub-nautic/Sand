using System;
using System.Collections;
using System.Collections.Generic;
using Playground.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playground.Player.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private HandSide handSide;
        [SerializeField] private Transform raycastInteractPoint;
        [SerializeField] private RaycastInteractor raycastInteractor;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private float maxRaycastDistance = 25f;

        private bool initialized;
        private void Start()
        {
            if (handSide == HandSide.Right)
            {
                InputProvider.MainInputActions.RightHandInteraction.Interact.performed += OnInteractionPerformed;
                InputProvider.MainInputActions.RightHandInteraction.Interact.canceled += OnInteractionCanceled;
            }
            else
            {
                InputProvider.MainInputActions.LeftHandInteraction.Interact.performed += OnInteractionPerformed;
                InputProvider.MainInputActions.LeftHandInteraction.Interact.canceled += OnInteractionCanceled;
            }
        }

        private void Awake()
        {
            // Initialize raycast interactor
            raycastInteractor = new RaycastInteractor(raycastInteractPoint, raycastLayerMask, maxRaycastDistance, true, handSide);

            initialized = true;
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            // Get interaction amount
            float interactionAmount = InputProvider.MainInputActions.LeftHandInteraction.InteractValue.ReadValue<float>();

            raycastInteractor.UpdateRaycastInteractor(interactionAmount);
        }

        private void OnInteractionPerformed(InputAction.CallbackContext context)
        {
            raycastInteractor.StartRaycastInteraction();
        }

        private void OnInteractionCanceled(InputAction.CallbackContext context)
        {
            raycastInteractor.EndRaycastInteraction();
        }

        private void OnDisable()
        {
            if (handSide == HandSide.Right)
            {
                InputProvider.MainInputActions.RightHandInteraction.Interact.performed -= OnInteractionPerformed;
                InputProvider.MainInputActions.RightHandInteraction.Interact.canceled -= OnInteractionCanceled;
            }
            else
            {
                InputProvider.MainInputActions.LeftHandInteraction.Interact.performed -= OnInteractionPerformed;
                InputProvider.MainInputActions.LeftHandInteraction.Interact.canceled -= OnInteractionCanceled;
            }
        }
    }
}