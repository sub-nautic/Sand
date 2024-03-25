using OverlordVR.Unit;
using UnityEngine;

namespace OverlordVR.Player.Usable
{
    public interface IUsable
    {
        int GetNeededCoins { get; }
        Transform GetUsableTransform { get; }

        void TryActive(Pouch pouch);
        void Active();
        void Cancel();
    }
}