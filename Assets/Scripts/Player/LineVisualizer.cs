using System;
using System.Collections;
using System.Collections.Generic;
using Playground.Player.Interaction;
using UnityEngine;

namespace Playground.Player
{
    public class LineVisualizer : MonoBehaviour
    {
        [SerializeField] private InteractionController controllerInteractor;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform pointerTransform;

        private bool pointerEnabled;

        private const float MAX_LENGTH = 25f;
        private const float RAY_OFFSET = 0.05f;
        private const int STEPS = 3;

        private void Update()
        {
            if (!controllerInteractor.Initialized)
            {
                return;
            }
            pointerEnabled = controllerInteractor.RaycastInteractor.IsHovering && !controllerInteractor.RaycastInteractor.IsInteracting; // Debug.Log($"[LineVisualizer] Update IsHovering: {controllerInteractor.RaycastInteractor.IsHovering}, IsInteracting: {!controllerInteractor.RaycastInteractor.IsInteracting}");

            SetPointerEnabled(pointerEnabled);
            if (!pointerEnabled)
            {
                return;
            }

            InteractableData hoverInteraction = controllerInteractor.RaycastInteractor.CurrentHoverInteractableData;
            UpdateLineRenderer(hoverInteraction);
            UpdatePointer(hoverInteraction);
        }

        private void UpdateLineRenderer(InteractableData hoverInteraction)
        {
            Vector3 origin = controllerInteractor.RaycastInteractorPosition;
            Vector3 direction = controllerInteractor.RaycastInteractorDirection * Mathf.Min(hoverInteraction.Distance, MAX_LENGTH);

            Vector3 end = origin + (direction - direction * RAY_OFFSET);

            for (int step = 0; step < STEPS; step++)
            {
                // Add evenly spaced vertices to the line renderer
                float progress = step / (float)(STEPS - 1);
                Vector3 position = Vector3.Lerp(origin, end, progress);
                lineRenderer.SetPosition(step, position);
            }
        }

        private void UpdatePointer(InteractableData hoverInteraction)
        {
            pointerTransform.position = hoverInteraction.HitPosition;
        }

        private void SetPointerEnabled(bool pointerEnabled)
        {
            lineRenderer.enabled = pointerEnabled;
            pointerTransform.position = enabled ? pointerTransform.position : new Vector3(0, -999f, 0);
        }
    }
}
