using System;
// using System.Collections.Generic;

[Serializable]
public class Game
{
    public string GameId;
    public string IsPlayerWon;
    public string PlayerId;
    public string ArenaId;

    public Game(string isPlayerWon, string playerId, string arenaId)
    {
        IsPlayerWon = isPlayerWon;
        PlayerId = playerId;
        ArenaId = arenaId;
    }

    // Propiedades de navegación
    // public Player Player { get; set; }
    // public Arena Arena { get; set; }
    // public List<Play> Plays { get; set; }
}

public class GameIdClass
{
    public string GameId;
}