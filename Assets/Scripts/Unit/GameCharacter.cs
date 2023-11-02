using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    [SerializeField] private UnitMovementController unitMovementController;

    public Action<Vector3> MovementInteractionEvent;

    private void Awake()
    {
        MovementInteractionEvent += OnInteractionEvent;
    }

    private void OnInteractionEvent(Vector3 position)
    {
        // Debug.Log($"[GameCharacter] Position to move: {position}");
        unitMovementController.StartMoveAction(position, 1);
    }
}
