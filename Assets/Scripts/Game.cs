using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlordVR.Unit
{
    public class Game : MonoBehaviour, IGame
    {
        public event Action StartGameEvent;

        public IEnumerable<GameUnit> GameUnits => gameUnits;
        public GameCharacter GameCharacter => gameCharacter;
        public bool HasGameStarted => hasGameStarted;

        private GameCharacter gameCharacter;
        private List<GameUnit> gameUnits = new();
        private bool hasGameStarted;
        private bool initialized;

        private void Start()
        {
            // To refactor
            SetGameCharacter();
        }

        public void StartGame()
        {

        }

        private void SetGameCharacter()
        {
            gameCharacter = FindObjectOfType<GameCharacter>();
        }
    }
}