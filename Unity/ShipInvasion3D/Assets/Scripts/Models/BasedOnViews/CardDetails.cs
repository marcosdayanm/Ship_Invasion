using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
        defenceCards.BalanceCards();
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

    public void BalanceCards(){
        // Diccionario para definir cuántas veces se debe duplicar cada objeto basado en su ID
        Dictionary<int, int> duplicaciones = new Dictionary<int, int>
        {
            { 1, 9 },
            { 2, 12 }, 
            { 3, 12 }, 
            { 4, 9 }, 
            { 5, 9 }, 
            { 6, 6 }, 
            { 7, 6 }, 
            { 8, 6 }, 
            { 9, 6 }, 
            { 10, 4 }, 
            { 11, 4 },  
            { 12, 3 }, 
            { 13, 4 }, 
            { 14, 4 }, 
            { 15, 3 }, 
            { 16, 3 }, 
            { 17, 2 }, 
            { 18, 2 }, 
            { 19, 2 }, 
            { 20, 2 }, 
            { 21, 1 }, 
            { 22, 1 }
        };

        // Crear una nueva lista para almacenar los resultados
        List<CardDetails> balancedCards = new List<CardDetails>();

        foreach (CardDetails card in this.Items){
            // Añadir el número de copias definidas en el diccionario
            for (int i = 0; i < duplicaciones[card.CardId]; i++){
                // Debug.Log(card.CardId);
                balancedCards.Add(card);
            }
        }

        this.Items = balancedCards;
    }
}
