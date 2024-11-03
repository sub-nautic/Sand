using OverlordVR.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverlordVR.Player.Interaction
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
        private InputAction currentHandInteraction;

        private void Awake()
        {
            // Subscribe to interaction input events based on hand side.
            if (handSide == HandSide.Right)
            {
                InputProvider.MainInputActions.XRIRightInteraction.Activate.performed += OnInteractionPerformed;
                InputProvider.MainInputActions.XRIRightInteraction.Activate.canceled += OnInteractionCanceled;
                currentHandInteraction = InputProvider.MainInputActions.XRIRightInteraction.ActivateValue;
            }
            else
            {
                InputProvider.MainInputActions.XRILeftInteraction.Activate.performed += OnInteractionPerformed;
                InputProvider.MainInputActions.XRILeftInteraction.Activate.canceled += OnInteractionCanceled;
                currentHandInteraction = InputProvider.MainInputActions.XRILeftInteraction.ActivateValue;
            }

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
            float interactionAmount = currentHandInteraction.ReadValue<float>();

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
                InputProvider.MainInputActions.XRIRightInteraction.Activate.performed -= OnInteractionPerformed;
                InputProvider.MainInputActions.XRIRightInteraction.Activate.canceled -= OnInteractionCanceled;
            }
            else
            {
                InputProvider.MainInputActions.XRILeftInteraction.Activate.performed -= OnInteractionPerformed;
                InputProvider.MainInputActions.XRILeftInteraction.Activate.canceled -= OnInteractionCanceled;
            }
        }
    }
}