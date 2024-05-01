using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Este script funciona para dar cartas al jugador en modo combate y en modo preparación


public class GiveCards : MonoBehaviour
{
    // Referencia al controlador del juego
    GameController gameController;

    // Prefabs de las manos del jugador en modo preparación y combate para ir añadiendo las cartas disponibles para el juego
    [SerializeField] GameObject playerHandPreparation;
    [SerializeField] GameObject playerHandCombat;

    // Prefab de la celda de la carta (lugar de una carta en la mano del jugador)
    [SerializeField] GameObject cellCard;

    // Variable para controlar si se están dando cartas (porque, por la animación tarda un poco en terminar de repartir cartas)
    bool isGivingCards = false;


    // Start is called before the first frame update
    void Start()
    {
        // Buscamos el controlador del juego para poder acceder a las cartas
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Función para dar una sola carta al jugador en modo combate
    public void GiveCardInCombatMode(){
        // Si la mano del jugador no tiene 5 cartas y no se están dando cartas, se reparte 1 más
        if(playerHandCombat.transform.childCount < 5 && !isGivingCards){
            // Obtenemos las cartas disponibles que tenemos guardadas en el gameController
            Cards cards = gameController.cards;
            // Llamamos a la corrutina para repartir las cartas (animación de repartir cartas)
            StartCoroutine(GiveCardsCoroutine(cards, playerHandCombat, amountCarts: 1));
        }
    }

    // Función para dar cartas al jugador en modo combate
    public void GiveCardsInCombatMode(){
        // Si la mano del jugador no tiene 5 cartas y no se están dando cartas, se reparten
        if(playerHandCombat.transform.childCount < 5 && !isGivingCards){
            // Obtenemos las cartas disponibles que tenemos guardadas en el gameController
            Cards cards = gameController.cards;
            // Llamamos a la corrutina para repartir las cartas (animación de repartir cartas)
            StartCoroutine(GiveCardsCoroutine(cards, playerHandCombat));
        }
    }

    // Función para dar cartas al jugador en modo preparación
    public void GiveCardsInPreparationMode(){
        // Si la mano del jugador no tiene 5 cartas, se reparten
        if(playerHandPreparation.transform.childCount < 5){
            // Obtenemos únicamente las cartas de defensa disponibles que tenemos guardadas en el gameController
            Cards defenseCards = gameController.cards.DefenseCards();
            // Llamamos a la corrutina para repartir las cartas (animación de repartir cartas)
            StartCoroutine(GiveCardsCoroutine(defenseCards, playerHandPreparation, true));
        }
    }

    // Corrutina para dar cartas al jugador (animación de repartir cartas)
    public IEnumerator GiveCardsCoroutine(Cards cards, GameObject playerHand, bool prepatationMode = false, int amountCarts = 5){
        // Indicamos que se están dando cartas
        isGivingCards = true;
        // Esperamos un segundo antes de empezar a repartir cartas (para que no sea instantáneo)
        yield return new WaitForSeconds(0.5f);
        // Repartimos 5 cartas
        for(int i = 0; i < amountCarts; i++){
            // Obtenemos un índice aleatorio para obtener una carta aleatoria de todas las disponibles
            int randomIndex = Random.Range(0, cards.Items.Count);
            // Instanciamos una celda de carta en la mano del jugador
            GameObject newCardCell = Instantiate(cellCard, playerHand.transform);
            // Obtenemos el gameObject de la carta que acabamos de instanciar para poder cambiarle el sprite
            Transform cardObject = newCardCell.transform.Find("Card");
            // Guardamos la carta random que hemos obtenido
            CardDetails currentCard = cards.Items[randomIndex];
            // Asignamos la información correspodiente de la carta al objeto de la carta
            cardObject.GetComponent<CardController>().cardDetails = currentCard;
            // Asignamos el sprite de la carta al objeto de la carta (dependiendo si es de defensa o de ataque)
            if(currentCard.CardType == "Defense"){
                cardObject.GetComponent<CardController>().image.sprite = Resources.Load<Sprite>("Images/CartasDefensa/" + currentCard.CardId.ToString());
            }else{
                cardObject.GetComponent<CardController>().image.sprite = Resources.Load<Sprite>("Images/CartasAtaque/" + currentCard.CardId.ToString());
            }
            // Esperamos medio segundo antes de repartir la siguiente carta (para que se vea un buen efecto)
            yield return new WaitForSeconds(0.3f);
        }
        gameController.cardsInHand = playerHand.transform.childCount;
        // Esperamos un segundo antes de terminar de repartir cartas
        yield return new WaitForSeconds(1f);
        // Si estamos en modo preparación, cambiamos al modo de defensa
        if(prepatationMode){
            // Pasamos a modo de defensa para que el jugador pueda usar las cartas de defensa y colocar sus barcos en el tablero
            gameController.DefenseMode();
            gameController.startCombatButton.gameObject.SetActive(true);
            gameController.playerHandPreparation.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Botones/18 copy");
        }
        // Indicamos que hemos terminado de dar cartas
        isGivingCards = false;
    }
}