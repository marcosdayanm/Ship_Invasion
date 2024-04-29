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

    // Prefab de un contenedor de una carta
    [SerializeField] GameObject cellCardPrefab;

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
        // Destruir el contenedor de la carta actual, porque ya no se va a ubicar ahí
        Destroy(cardController.parentToReturnTo.gameObject);
        // Cambiamos el padre de la carta a la zona de uso (panel) para que se quede ahí en lugar de volver a la mano
        cardController.parentToReturnTo = transform;
        // Desactivamos el texto que se muestra en el panel para que solo esté la carta
        text.SetActive(false);
        // Activamos la variable que indica que se está usando una carta
        gameController.isCardInUse = true;
        // Activamos el modo de ataque o defensa dependiendo del tipo de carta
        if(cardController.cardDetails.CardType == "Attack"){
            gameController.attackCardLength[0] = cardController.cardDetails.LengthX;
            gameController.attackCardLength[1] = cardController.cardDetails.LengthY;
            gameController.currentAttackCard = cardController.cardDetails;
            // Destuir la carta actual
            Destroy(card.gameObject);
            // Pasar a modo ataque
            gameController.SetAttackGridState();
            // Volvemos a activar el texto que se muestra en el panel para que solo esté la carta
            text.SetActive(true);
            // Activamos la variable que indica que se está usando una carta
            gameController.isCardInUse = false;
        }else{
            // Crear un nuevo contenedor para situar la carta que se está usuando
            GameObject cellCard = Instantiate(cellCardPrefab, playerHand.transform);
            // Destuir la carta que viene por defecto en el prefab (porque ya tenemos la carta que vamos a poner en el contenedor)
            Destroy(cellCard.transform.Find("Card").gameObject);
            // Asignar como padre al nuevo contenedor que acabamos de crear
            cardController.parentToReturnTo = cellCard.transform;
            // Pasar a modo defensa
            gameController.SetDefenseGridState();
            // Volvemos a activar el texto que se muestra en el panel para que solo esté la carta
            text.SetActive(true);
            // Activamos la variable que indica que se está usando una carta
            gameController.isCardInUse = false;
        }
    }
}
