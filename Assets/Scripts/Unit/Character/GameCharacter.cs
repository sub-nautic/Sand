using System;
using UnityEngine;

namespace OverlordVR.Unit
{
    public class GameCharacter : MonoBehaviour
    {
        [SerializeField] private UnitMovementController unitMovementController;

        public Action<Vector3> MovementInteractionEvent;

        private const float DEFAULT_SPEED = 1f;

        private void Awake()
        {
            MovementInteractionEvent += OnInteractionEvent;
        }

        private void OnInteractionEvent(Vector3 position)
        {
            unitMovementController.StartMoveAction(position, DEFAULT_SPEED);
        }
    }
}
