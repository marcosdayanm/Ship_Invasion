using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script para controlar las acciones del CPU, métodos que se llamarán desde el gamecontroller que interactuarán con los quads de los grids, y con la acción de lanzar proyectiles

public class BotCPU : MonoBehaviour
{
    // Referencia al controlador del juego
    GameController gameController;
    [SerializeField] GridStateController gridStateControllerBot;    
    [SerializeField] GridStateController gridStateControllerPlayer;    

    List<CardDetails> deck = new List<CardDetails>();


    // Start is called before the first frame update
    void Start()
    {
        // Buscamos el controlador del juego para poder acceder a las cartas
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        gridStateControllerBot = GameObject.FindWithTag("EnemyGrid").GetComponent<GridStateController>();
        gridStateControllerPlayer = GameObject.FindWithTag("Grid").GetComponent<GridStateController>();
    }


    // Función para dar una sola carta al jugador en modo combate
    public void GiveCardInCombatMode(){
        if(deck.Count < 5){
            Cards cards = gameController.cards;
            GiveCards(cards, amountCarts: 1);
        }
    }

    // Función para dar cartas al jugador en modo combate
    public void GiveCardsInCombatMode(){
        if(deck.Count < 5){
            Cards cards = gameController.cards;
            GiveCards(cards);
        }
    }

    // Función para dar cartas al jugador en modo preparación
    public void GiveCardsInPreparationMode(){
        if(deck.Count < 5){
            Cards cards = gameController.cards.DefenseCards();
            GiveCards(cards);
        }
    }

    // Función para alistar el estado de la partida del bot en Preparation Mode
    public void PreparationMode(){
        GiveCardsInPreparationMode();
        PlaceShips(true);
    }


    // Método para dar cartas al jugador (animación de repartir cartas)
    public void GiveCards(Cards cards, int amountCarts = 5){
        for(int i = 0; i < amountCarts; i++){
            int randomIndex = Random.Range(0, cards.Items.Count);
            deck.Add(cards.Items[randomIndex]);
        }
    }



    // Método para poner barcos en el grid del bot
    public void PlaceShips(bool isPreparationMode = false){

        if (isPreparationMode)
        {
            Transform grid = gridStateControllerBot.transform;

            foreach (CardDetails card in deck){
                int randomX = Random.Range(0, 12);
                int randomY = Random.Range(0, 12);
                Transform startingQuad = grid.GetChild(randomX).GetChild(randomY);

                if (gridStateControllerBot.ValidateShipPlacing(card, startingQuad))
                    gridStateControllerBot.PlaceShipMisile(card, startingQuad);
            }
        }
    }



}
