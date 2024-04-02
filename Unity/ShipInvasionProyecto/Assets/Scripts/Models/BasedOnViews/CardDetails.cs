using System;

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
}
