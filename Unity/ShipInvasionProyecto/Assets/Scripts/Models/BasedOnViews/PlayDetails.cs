using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayDetails
{
    public int PlayId;
    public int PlayNumber;
    public bool IsPlayerPlay;
    public int NumFieldsCovered;
    public int GameId;
    public int CardId;
    public string CardName;
    public string CardType;
    public string CardQuality;
    public int LengthX;
    public int LengthY;
    public DateTime GameDate;
    public bool GameIsPlayerWonGame;
}


[System.Serializable]
public class Plays
{
    public List<PlayDetails> Items;
}
