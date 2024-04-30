using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardConnection : MonoBehaviour
{
    private Cards cards;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private GameObject cardPrefab;
   void Start() {
    cards = JsonUtility.FromJson<Cards>(PlayerPrefs.GetString("cards")); //Convertir las cartas de string a json (objetos)
    cards = cards.SortCards();
    ShowCards();
   } 

   void ShowCards() {
    foreach (CardDetails card in cards.Items) { //Guardar en una varaiable temporal (card) cards.items
    GameObject newCard = Instantiate(cardPrefab, cardsContainer); //Instanciar dentro del card container
    if(card.CardType == "Defense"){
        newCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CartasDefensa/" + card.CardId.ToString());
    }else {
        newCard.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CartasAtaque/" + card.CardId.ToString());
        }
    } 
   }
}