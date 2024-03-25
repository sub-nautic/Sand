using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverlordVR.Unit
{
    public class Pouch : MonoBehaviour
    {
        [SerializeField] private int coinsAmount = 50;
        [SerializeField] private Coin coin;

        public int GetCoins => coinsAmount;

        //Get Coin
        public int GrabCoin()
        {
            return coinsAmount += 1;
        }

        // Que Grab Coins

        //Lost Coin
        public int ReduceCoin()
        {
            return coinsAmount -= 1;
        }

        //Que reduce Coins

        public void TransferCoin(Transform TransferTransform, out Coin newCoin)
        {
            ReduceCoin();
            newCoin = Instantiate(coin, transform.position, transform.rotation);
        }

        public void DropCoin()
        {

        }
    }
}