using System;
using System.Collections.Generic;


// Esta clase sirve para guardar los detalles de un jugador y poder serializarla y deserializarla
[System.Serializable] public class PlayerDetails{
    public int PlayerId;
    public string PlayerUsername;
    public DateTime PlayerCreationDate;
    public int PlayerWins;
    public int PlayerLosses;
    public int PlayerCoins;

    // public PurchasedSprite PurchasedSpritePlayerId; 
    // public PurchasedSprite PurchasedSpriteSpriteId;
    // public List<PurchasedSpriteDetails> PurchasedSprites { get; set; }
}

// Esta clase sirve para guardar una lista de jugadores y poder serializarla y deserializarla
[System.Serializable] public class Players {
    public List<PlayerDetails> Items;
}

public class EditIsPlayerWon
{
    public string IsPlayerWon = "1";
}


public class EditPlayerDetails 
{
    public int PlayerId; 
    public int PlayerCoins;
    public int PlayerWins;
    public int PlayerLosses;

    public EditPlayerDetails(int PlayerId, int PlayerCoins, int PlayerWins, int PlayerLosses)
    {
        this.PlayerId = PlayerId;
        this.PlayerCoins = PlayerCoins;
        this.PlayerWins = PlayerWins;
        this.PlayerLosses = PlayerLosses;
    }
}