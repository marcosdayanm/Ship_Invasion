using System;
using System.Collections.Generic;


// Esta clase sirve para guardar los detalles de una jugada y poder serializarla y deserializarla
[System.Serializable] public class PlayDetails{
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


// Esta clase sirve para guardar una lista de jugadas y poder serializarla y deserializarla
[System.Serializable] public class Plays{
    public List<PlayDetails> Items;
}
