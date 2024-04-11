using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCards : MonoBehaviour
{
    GameController gameController;
    [SerializeField] GameObject playerHandPreparation;
    [SerializeField] GameObject playerHandCombat;
    [SerializeField] GameObject cellCard;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveCardsInCombatMode(){
        Cards cards = gameController.cards;
        StartCoroutine(GiveCardsCoroutine(cards, playerHandCombat));
    }

    public void GiveCardsInPreparationMode(){
        Cards defenseCards = gameController.cards.DefenseCards();
        StartCoroutine(GiveCardsCoroutine(defenseCards, playerHandPreparation));
    }

    public IEnumerator GiveCardsCoroutine(Cards cards, GameObject playerHand){
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < 5; i++){
            int randomIndex = Random.Range(0, cards.Items.Count);
            GameObject newCardCell = Instantiate(cellCard, playerHand.transform);
            Transform cardObject = newCardCell.transform.Find("Card");
            CardDetails currentCard = cards.Items[randomIndex];
            cardObject.GetComponent<CardController>().cardDetails = currentCard;
            if(currentCard.CardType == "Defense"){
                cardObject.GetComponent<CardController>().image.sprite = Resources.Load<Sprite>("Images/CartasDefensa/" + currentCard.CardId.ToString());
            }else{
                cardObject.GetComponent<CardController>().image.sprite = Resources.Load<Sprite>("Images/CartasAtaque/" + currentCard.CardId.ToString());
            }
            gameController.CardsCounter++;
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        gameController.DefenseMode();
    }
}