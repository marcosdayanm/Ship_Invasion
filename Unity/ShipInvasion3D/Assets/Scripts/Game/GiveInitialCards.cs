using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveInitialCards : MonoBehaviour
{
    GameController gameController;
    [SerializeField] GameObject playerHand;
    [SerializeField] GameObject cellCard;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        Cards defenseCards = gameController.cards.DefenseCards();
        StartCoroutine(GiveCardsCoroutine(defenseCards));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GiveCardsCoroutine(Cards defenseCards){
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < 5; i++){
            int randomIndex = Random.Range(0, defenseCards.Items.Count);
            GameObject newCardCell = Instantiate(cellCard, playerHand.transform);
            Transform cardObject = newCardCell.transform.Find("Card");
            CardDetails currentCard = defenseCards.Items[randomIndex];
            cardObject.GetComponent<CardController>().cardDetails = currentCard;
            cardObject.GetComponent<CardController>().image.sprite = Resources.Load<Sprite>("Images/CartasDefensa/" + currentCard.CardId.ToString());
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        gameController.DefenseMode();
    }
}