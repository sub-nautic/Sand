using Playground.Player.Interaction;
using UnityEngine;

namespace Playground.Player
{
    public class LineVisualizer : MonoBehaviour
    {
        [Tooltip("The interaction controller responsible for raycasting")]
        [SerializeField] private InteractionController controllerInteractor;
        [Tooltip("The Line Renderer component used for visualization")]
        [SerializeField] private LineRenderer lineRenderer;

        private bool pointerEnabled;

        private const float MAX_LENGTH = 6f;
        private const float RAY_OFFSET = 0.05f;
        private const int STEPS = 3;

        private void Update()
        {
            if (!controllerInteractor.Initialized)
            {
                return;
            }

            // Check if the pointer is enabled
            pointerEnabled = controllerInteractor.RaycastInteractor.IsHovering; // && !controllerInteractor.RaycastInteractor.IsInteracting;

            SetLineEnabled(pointerEnabled);
            if (!pointerEnabled)
            {
                return;
            }

            // Get the currently hovered interaction data
            InteractableData hoverInteraction = controllerInteractor.RaycastInteractor.CurrentHoverInteractableData;

            // Update the line renderer based on the hover interaction
            UpdateLineRenderer(hoverInteraction);
        }

        private void UpdateLineRenderer(InteractableData hoverInteraction)
        {
            // The starting point of the line and ray direction.
            Vector3 origin = controllerInteractor.RaycastInteractorPosition;
            Vector3 direction = controllerInteractor.RaycastInteractorDirection * Mathf.Min(hoverInteraction.Distance, MAX_LENGTH);

            // Calculate the end point of the line
            Vector3 end = origin + (direction - direction * RAY_OFFSET);

            for (int step = 0; step < STEPS; step++)
            {
                // Add evenly spaced vertices to the line renderer
                float progress = step / (float)(STEPS - 1);
                Vector3 position = Vector3.Lerp(origin, end, progress);
                lineRenderer.SetPosition(step, position);
            }
        }

        private void SetLineEnabled(bool pointerEnabled)
        {
            // Enable or disable the line renderer based on the pointer's status
            lineRenderer.enabled = pointerEnabled;
        }
    }
}
