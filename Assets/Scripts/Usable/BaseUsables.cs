using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OverlordVR.Player.Usable;
using OverlordVR.Unit;
using UnityEngine;

namespace OverlordVR.Buildings
{
    public class BaseUsable : MonoBehaviour, IUsable
    {
        [SerializeField] private int neededCoins = 5;

        public int GetNeededCoins => neededCoins;
        public Transform GetUsableTransform => transform;

        private CancellationTokenSource tryAddCoinsCancelationTokenSource;
        private int addedCoins;

        public virtual void TryActive(Pouch pouch)
        {
            // try adding coins
            tryAddCoinsCancelationTokenSource = new();
            TryAddCoins(pouch, tryAddCoinsCancelationTokenSource.Token).Forget();
        }

        public virtual void Active()
        {
        }

        public virtual void Cancel()
        {
            // return coins
            // drop coins in random directions
        }

        private async UniTask TryAddCoins(Pouch pouch, CancellationToken cancellationToken = default)
        {
            if (pouch.GetCoins >= 1)
            {
                pouch.TransferCoin(GetUsableTransform, out Coin coin);
                addedCoins += 1;
            }
            else
            {
                //Cancel
            }

            await UniTask.Delay(500, false, PlayerLoopTiming.Update, cancellationToken);
        }
    }
}
