using UnityEngine;

namespace Playground.Player.Interaction
{
    public class InteractableData
    {
        public IInteractable Interactable { get; private set; }
        public float Distance { get; set; }
        public Vector3 HitPosition { get; set; }

        public InteractableData(IInteractable interactable, float distance, Vector3 hitPosition)
        {
            this.Interactable = interactable;
            this.Distance = distance;
            this.HitPosition = hitPosition;
        }
    }
}