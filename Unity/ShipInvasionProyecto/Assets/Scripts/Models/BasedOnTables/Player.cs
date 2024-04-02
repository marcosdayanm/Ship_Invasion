using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public int Id;
    public string Username;
    public string Password;
    public DateTime CreationDate;
    public int Wins;
    public int Losses;
    public int Coins;

    // Propiedades de navegación
    public List<PurchasedSprite> PurchasedSprites;
    public List<Game> Games;
}
