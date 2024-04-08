using System;
using System.Collections.Generic;


[System.Serializable]
public class PlayerDetails
{
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


[System.Serializable]
public class Players
{
    public List<PlayerDetails> Items;
}

