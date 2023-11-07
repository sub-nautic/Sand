using OverlordVR.Player.Usable;
using UnityEngine;

namespace OverlordVR.Buildings
{
    public class BaseUsable : MonoBehaviour, IUsable
    {
        public int NeededCoins { get; set; }

        public virtual void TryActive()
        {
            // try adding coins
        }

        public virtual void Active()
        {
        }

        public virtual void Cancel()
        {
            // return coins
        }
    }
}
