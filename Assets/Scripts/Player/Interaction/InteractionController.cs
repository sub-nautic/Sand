using Playground.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playground.Player.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        [Tooltip("The hand side associated with this controller (left or right)")]
        [SerializeField] private HandSide handSide;
        [Tooltip("The transform representing the point from which raycasting originates")]
        [SerializeField] private Transform raycastInteractPoint;
        [Tooltip("The layer mask for raycasting to specific layers")]
        [SerializeField] private LayerMask raycastLayerMask;
        [Tooltip("The maximum distance for raycasting")]
        [SerializeField] private float maxRaycastDistance = 25f;

        public Vector3 RaycastInteractorPosition => raycastInteractPoint.position;
        public Vector3 RaycastInteractorDirection => raycastInteractPoint.transform.forward;
        public bool Initialized => initialized;
        public RaycastInteractor RaycastInteractor => raycastInteractor;

        private bool initialized;
        private RaycastInteractor raycastInteractor;

        private void Start()
        {
            // Subscribe to interaction input events based on hand side.
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
            raycastInteractor = new RaycastInteractor(raycastInteractPoint, raycastLayerMask, maxRaycastDistance, true);

            initialized = true;
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            // Get interaction amount from input.
            float interactionAmount = InputProvider.MainInputActions.LeftHandInteraction.InteractValue.ReadValue<float>();

            raycastInteractor.UpdateRaycastInteractor(interactionAmount);
        }

        private void OnInteractionPerformed(InputAction.CallbackContext context)
        {
            // Handle the start of raycast interaction.
            raycastInteractor.StartRaycastInteraction();
        }

        private void OnInteractionCanceled(InputAction.CallbackContext context)
        {
            // Handle the end of raycast interaction.
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