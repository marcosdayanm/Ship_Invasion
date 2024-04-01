using System;
using System.Collections.Generic;

[Serializable]
public class Sprite
{
    public int Id;
    public string Name;
    public bool IsAddOn;
    public int Price;

    // Propiedades de navegación
    public List<PurchasedSprite> PurchasedSprites { get; set; }
    public List<Card> CardSkins { get; set; }
    public List<Card> CardEffects { get; set; }
    public List<Arena> Arenas { get; set; }
}
