using System;
using System.Collections.Generic;
using System.Linq;


// Esta clase sirve para guardar los detalles de una carta y poder serializarla y deserializarla
[System.Serializable] public class CardDetails{
    public int CardId;
    public string CardName;
    public string CardType;
    public string CardQuality;
    public int LengthX;
    public int LengthY;

    // public SpriteDetails Skin;
    // public SpriteDetails Effect;
}

// Esta clase sirve para guardar una lista de cartas y poder serializarla y deserializarla
[System.Serializable] public class Cards{
    public List<CardDetails> Items;

    // Este es un método que sirve para filtrar las cartas de tipo defensa
    public Cards DefenseCards(){
        Cards defenceCards = new Cards();
        defenceCards.Items = this.Items.Where(card => card.CardType == "Defense").ToList();
        return defenceCards;
    }

    // Este es un método que sirve para filtrar las cartas de tipo ataque
    public Cards AttackCards(){
        Cards attackCards = new Cards();
        attackCards.Items = this.Items.Where(card => card.CardType == "Attack").ToList();
        return attackCards;
    }

    // Este es un método que sirve para ordenar las cartas por tipo de ataque
    public Cards SortCards(){
        Cards sortedCards = new Cards();
        sortedCards.Items = this.Items.OrderBy(obj => obj.CardId).ToList();
        return sortedCards;
    }
}
