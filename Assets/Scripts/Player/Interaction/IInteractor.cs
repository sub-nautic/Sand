using UnityEngine;

namespace OverlordVR.Player.Interaction
{
    public interface IInteractor
    {
        Transform Transform { get; }
        Vector3 HitPosition { get; }
        Vector3 HoverHitPosition { get; }
        float InteractionAmount { get; }
        HandSide HandSide { get; }
    }
}