using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Este script sirve para que cuando se arrastre una carta a la zona de uso, se active el modo de ataque o defensa dependiendo del tipo de carta


public class UseCard : MonoBehaviour, IDropHandler
{
    // Referencia al texto que se muestra en el panel cuando se puede usar una carta
    [SerializeField] GameObject text;
    // Referencia al controlador del juego
    GameController gameController;

    // Referencia al contenedor de la mano de preparación (o de defensa)
    [SerializeField] private GameObject playerHand;

    void Start()
    {
        // Buscamos el controlador del juego para poder acceder a las cartas
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    // Cuando se suelta una carta en la zona de uso (panel), se activa el modo de ataque o defensa dependiendo del tipo de carta
    public void OnDrop(PointerEventData eventData)
    {
        // Obtenemos la carta que se ha soltado en la zona de uso (panel)
        GameObject card = eventData.pointerDrag;
        // Obtenemos el controlador de la carta
        CardController cardController = card.GetComponent<CardController>();
        // Cambiamos el padre de la carta a la zona de uso (panel) para que se quede ahí en lugar de volver a la mano
        cardController.parentToReturnTo = transform;
        // Desactivamos el texto que se muestra en el panel para que solo esté la carta
        text.SetActive(false);
        // Activamos la variable que indica que se está usando una carta
        gameController.isCardInUse = true;
        // Activamos el modo de ataque o defensa dependiendo del tipo de carta
        if(cardController.cardDetails.CardType == "Attack"){
            gameController.SetAttackGridState();
        }else{
            cardController.parentToReturnTo = playerHand.transform;
            gameController.SetDefenseGridState();
        }
    }
}
