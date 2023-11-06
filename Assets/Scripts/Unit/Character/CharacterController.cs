using System.Collections.Generic;
using OverlordVR.Input;
using OverlordVR.Player.Usable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverlordVR.Unit
{
    public class CharacterController : MonoBehaviour
    {
        private List<IUsable> usables = new();

        private void Awake()
        {
            InputProvider.MainInputActions.RightHandInteraction.PrimaryButton.performed += OnRightPrimaryButtonPerformed;
            InputProvider.MainInputActions.RightHandInteraction.PrimaryButton.canceled += OnRightPrimaryButtonCanceled;
        }

        private void OnRightPrimaryButtonPerformed(InputAction.CallbackContext context)
        {
            Debug.Log($"[CharacterController] OnRightPrimaryButtonPerformed");
        }

        private void OnRightPrimaryButtonCanceled(InputAction.CallbackContext context)
        {
            Debug.Log($"[CharacterController] OnRightPrimaryButtonCanceled");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IUsable>(out IUsable usable))
            {
                Debug.Log($"[CharacterController] Found IUsable");
                usables.Add(usable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IUsable>(out IUsable usable) && usables.Contains(usable))
            {
                Debug.Log($"[CharacterController] Found IUsable");
                usables.Remove(usable);
            }
        }

        private void OnDisable()
        {
            InputProvider.MainInputActions.RightHandInteraction.PrimaryButton.performed -= OnRightPrimaryButtonPerformed;
            InputProvider.MainInputActions.RightHandInteraction.PrimaryButton.canceled -= OnRightPrimaryButtonCanceled;
        }
    }
}
