using System;
using System.Collections.Generic;

public interface IGame
{
    event Action StartGameEvent;
    IEnumerable<GameUnit> GameUnits { get; }
    bool HasGameStarted { get; }

    void StartGame();
}
