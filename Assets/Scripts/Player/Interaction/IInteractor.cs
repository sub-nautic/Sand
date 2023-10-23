using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playground.Player.Interaction
{
    public interface IInteractor
    {
        Transform Transform { get; }
        float InteractionAmount { get; }
        HandSide HandSide { get; }
    }
}