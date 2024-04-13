using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UseCard : MonoBehaviour, IDropHandler
{
    [SerializeField] GameObject text;
    GameController gameController;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject card = eventData.pointerDrag;
        CardController cardController = card.GetComponent<CardController>();
        cardController.lastCellCard = cardController.parentToReturnTo;
        cardController.parentToReturnTo = transform;
        text.SetActive(false);
        gameController.isCardInUse = true;
        if(cardController.cardDetails.CardType == "Attack"){
            gameController.AtackMode();
        }else{
            gameController.DefenseMode();
        }
    }
}
