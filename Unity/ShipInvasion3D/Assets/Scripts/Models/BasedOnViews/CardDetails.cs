using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class CardDetails
{
    public int CardId;
    public string CardName;
    public string CardType;
    public string CardQuality;
    public int LengthX;
    public int LengthY;

    // public SpriteDetails Skin;
    // public SpriteDetails Effect;
}

[System.Serializable]
public class Cards
{
    public List<CardDetails> Items;

    public Cards DefenseCards()
    {
        Cards defenceCards = new Cards();
        defenceCards.Items = this.Items.Where(card => card.CardType == "Defense").ToList();
        return defenceCards;
    }

    public Cards AttackCards()
    {
        Cards attackCards = new Cards();
        attackCards.Items = this.Items.Where(card => card.CardType == "Attack").ToList();
        return attackCards;
    }
}
