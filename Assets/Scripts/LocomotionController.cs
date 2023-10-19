using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Playground.Player.Movement
{
    public class LocomotionController : MonoBehaviour
    {
        [SerializeField] private XROrigin playerXRRig;

        [Header("Controller movement")]
        [Tooltip("Speed when moving with the controller (not precise distance traveled per second)")]
        [SerializeField] private float controllerMovementSpeed = 2f;
        [Tooltip("Time before initiating a turn when holding the joystick")]
        [SerializeField] private float turnStartDelay = 0.75f;
        [Tooltip("Time between consecutive turns while holding the joystick")]
        [SerializeField] private float turnRepeatDelay = 0.5f;
        [Tooltip("Enable turn smoothing (user-configurable in the future)")]
        [SerializeField] private float turnRotationAmount = 45f;
        [Tooltip("Whether we want to use turn smoothing. Later this should be configurable by the user.")]
        [SerializeField] private bool useTurnSmoothing = true;
        [Tooltip("Time it takes for the turn to complete (keep it low to prevent motion sickness)")]
        [SerializeField] private float turnSmoothTime = 0.02f;

        [Header("Vertical movement")]
        [Tooltip("Transition time when stepping on higher surfaces, multiplied by the actual step height")]
        [SerializeField] private float stepTransitionTime = 0.5f;
        [Tooltip("Maximum step height when walking onto colliders")]
        [SerializeField] private float maxStepHeight = 0.4f;
        [Tooltip("Maximum step height above which you will fall instead of stepping")]
        [SerializeField] private float maxStepFallHeight = 1.0f;
        [Tooltip("Distance above the ground that the player rig will still be considered grounded")]
        [SerializeField] private float groundedDistance = 0.02f;
        [Tooltip("Maximum radius of holes you can step over without falling")]
        [SerializeField] private float maxStepOverRadius = 0.05f;
        [Tooltip("Maximum falling speed allowed")]
        [SerializeField] private float maxFallSpeed = 55.55f;
        [Tooltip("Y position below which the player will be teleported back to the starting position")]
        [SerializeField] private float resetPositionYThreshold = -50f;
        [Tooltip("Maximum raycast distance when detecting the ground")]
        [SerializeField] private float maxGroundRaycastDistance = 4f;
        [Tooltip("Define if player can walk horizontal, turning off gives fly mode")]
        [SerializeField] private bool horizontalMovement = true;

        [Header("Obstacle detection")]
        [Tooltip("Vertical distance from the floor to the bottom point of the capsule cast; obstacles below this height won't be detected")]
        [SerializeField] private float capsuleBottomHeight = 0.3f;
        [Tooltip("Radius of the body (capsule cast radius)")]
        [SerializeField] private float capsuleRadius = 0.1f;
        [Tooltip("Distance to maintain between the body (headset) and obstacles when trying to move into obstacles")]
        [SerializeField] private float obstacleMargin = 0.1f;
        [Tooltip("Minimum speed required to initiate sliding on a wall")]
        [SerializeField, Range(0f, 1f)] private float minSlideSpeed = 0.1f;
        [Tooltip("Multiplier applied when calculating slide speed")]
        [SerializeField, Min(1f)] private float slideMultiplier = 1.4f;
        [SerializeField] private LayerMask movementRaycastLayers = ~0;

        private MainInputActions inputActions;
        private float turnDirection = 0f;
        private float turnPerformedTime = 0f;
        private float turnTargetAngle = 0f;
        private float turnCurrentAngle = 0f;
        private float turnCurrentAngleVelocity = 0f;
        private float targetHeightVelocity;
        private Vector3 currentYVelocity;
        private bool isFalling = false;
        // private bool isGrounded;
        private Vector3 startPosition;

        private bool isInitialized;

        private const float GROUND_RAYCAST_HEIGHT_OFFSET = 1.5f;

        private void Awake()
        {
            inputActions = new();
            inputActions.Enable();

            inputActions.RightHandLocomotion.Turn.performed += OnTurnPerformed;
            inputActions.RightHandLocomotion.Turn.canceled += OnTurnCancelled;

            startPosition = transform.position;

            isInitialized = true;
        }

        private void OnTurnPerformed(InputAction.CallbackContext context)
        {
            TurnPlayerDirection(context.ReadValue<Vector2>().x);
            Debug.Log($"[LocomotionController] OnTurnPerformed {context.ReadValue<Vector2>().x}");
        }
        private void OnTurnCancelled(InputAction.CallbackContext context)
        {
            turnDirection = 0f;
        }

        private void TurnPlayerDirection(float direction)
        {
            direction = Math.Sign(direction);

            if (useTurnSmoothing)
            {
                turnDirection = direction;

                if (direction != 0f)
                {
                    turnPerformedTime = Time.time + (turnStartDelay - turnRepeatDelay);
                    turnTargetAngle += turnDirection * turnRotationAmount;
                }
            }
            else
            {
                playerXRRig.RotateAroundCameraUsingOriginUp(direction * turnRotationAmount);
            }
        }

        private void UpdatePlayerMovement()
        {
            // Read input form controller
            Vector3 inputDirection = inputActions.LeftHandLocomotion.Move.ReadValue<Vector2>();

            // Calculate the amount of translation based on input and speed
            float movementAmount = inputDirection.magnitude * controllerMovementSpeed * Time.deltaTime;

            // Calculate the movement direction from a 2D input vector
            Vector3 inputVector = new Vector3(inputDirection.x, 0f, inputDirection.y).normalized;
            Vector3 movementDirection = playerXRRig.Camera.transform.TransformDirection(inputVector);

            // Combine translation for player movement
            Vector3 translation = movementDirection * movementAmount;

            // Calculate capsule points and perform a forward cast
            float capsuleCastDistance = movementAmount + obstacleMargin;
            Vector3 capsuleEnd = playerXRRig.Camera.transform.position;
            Vector3 capsuleStart = playerXRRig.Camera.transform.position;
            capsuleStart.y = playerXRRig.transform.position.y + capsuleBottomHeight;

            bool isAboveFloorHeight = capsuleEnd.y > capsuleStart.y;
            if (isAboveFloorHeight && Physics.CapsuleCast(capsuleStart, capsuleEnd, capsuleRadius, movementDirection, out RaycastHit hit, capsuleCastDistance, movementRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                // Calculate the slide direction and amount based on the angle to the hit normal
                Vector3 slideTranslation = SlideDirection(movementAmount, movementDirection, ref hit);

                // Calculate the forward translation amount by determining how much more we can move in the forwards direction without intersecting a collider
                Vector3 forwardTranslation = Mathf.Clamp(hit.distance - capsuleRadius, 0f, movementAmount) * movementDirection;

                // Combine the results
                translation = forwardTranslation + slideTranslation;
            }

            MovePlayer(translation);
        }

        private void UpdatePlayerDirection()
        {
            // Perform turns at a fixed interval, while joystick is held
            if (turnDirection != 0f && (Time.time - turnPerformedTime) > turnRepeatDelay)
            {
                turnPerformedTime = Time.time;
                turnTargetAngle += turnDirection * turnRotationAmount;
            }

            // Smoothly turn to target angle
            if (turnCurrentAngle != turnTargetAngle)
            {
                float newCurrentAngle = Mathf.SmoothDamp(turnCurrentAngle, turnTargetAngle, ref turnCurrentAngleVelocity, turnSmoothTime);
                float delta = newCurrentAngle - turnCurrentAngle;
                turnCurrentAngle = newCurrentAngle;
                playerXRRig.RotateAroundCameraUsingOriginUp(delta);
            }
        }

        private Vector3 SlideDirection(float movementAmount, Vector3 movementDirection, ref RaycastHit hit)
        {
            float slideAmount = 1f - Mathf.Abs(Vector3.Dot(movementDirection, hit.normal));
            slideAmount = movementAmount * Mathf.Clamp(slideAmount * slideMultiplier, minSlideSpeed, 1f);
            Vector3 slideTranslation = slideAmount * Vector3.ProjectOnPlane(movementDirection, hit.normal).normalized;
            return slideTranslation;
        }

        private void MovePlayer(Vector3 deltaVector)
        {
            // Move on horizontal plane only. Disabling this basically gives us fly mode
            if (horizontalMovement)
            {
                deltaVector.y = 0f;
            }

            // Translate whole rig
            transform.position += deltaVector;
        }

        private void UpdatePlayerGrounded()
        {
            Vector3 rigPosition = playerXRRig.transform.position;
            Vector3 raycastOrigin = playerXRRig.transform.position + Vector3.up * GROUND_RAYCAST_HEIGHT_OFFSET;

            // Perform a sphere cast to detect the ground
            if (Physics.SphereCast(raycastOrigin, maxStepOverRadius, Vector3.down, out RaycastHit hit, maxGroundRaycastDistance, movementRaycastLayers))
            {
                // Calculate the relative floor level (positive if below the floor)
                float floorLevel = -(hit.distance - GROUND_RAYCAST_HEIGHT_OFFSET + maxStepOverRadius);

                // Check if we are below the floor but within the max step height
                if (floorLevel > 0f && floorLevel < maxStepHeight)
                {
                    // Adjust the player's position smoothly to take a step
                    rigPosition.y = Mathf.SmoothDamp(rigPosition.y, rigPosition.y + floorLevel, ref targetHeightVelocity, stepTransitionTime * floorLevel);
                }

                // Update falling state:
                // - If we are above the floor and falling (with a small margin when not falling), or
                // - If we are inside the floor
                isFalling = (floorLevel < 0f && isFalling) || floorLevel < -0.05f || floorLevel > maxStepFallHeight;

                // Update grounded state
                // isGrounded = floorLevel > -groundedDistance;
            }
            else
            {
                isFalling = true;
            }

            if (isFalling)
            {
                // Apply gravity to the current velocity
                currentYVelocity += Physics.gravity * Time.deltaTime;

                // Limit the velocity to the terminal velocity
                if (currentYVelocity.sqrMagnitude > maxFallSpeed * maxFallSpeed)
                {
                    currentYVelocity = currentYVelocity.normalized * maxFallSpeed;
                }

                // Apply the change to the player's position
                rigPosition += currentYVelocity * Time.deltaTime;

                // If the player falls below a certain Y position and is still falling, teleport them to the starting point
                if (rigPosition.y < resetPositionYThreshold)
                {
                    transform.position = startPosition;
                    return;
                }
            }
            else
            {
                currentYVelocity = Vector3.zero;
            }

            transform.position = rigPosition;
        }

        private void Update()
        {
            if (isInitialized)
            {
                UpdatePlayerDirection();
                UpdatePlayerGrounded();
                UpdatePlayerMovement();
            }
        }

        private void OnDestroy()
        {
            inputActions.RightHandLocomotion.Turn.performed -= OnTurnPerformed;
            inputActions.RightHandLocomotion.Turn.canceled -= OnTurnCancelled;
        }
    }
}
