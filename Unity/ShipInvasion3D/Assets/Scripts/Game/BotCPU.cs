using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Script para controlar las acciones del CPU, métodos que se llamarán desde el gamecontroller que interactuarán con los quads de los grids, y con la acción de lanzar proyectiles

public class BotCPU : MonoBehaviour
{
    // Referencia al controlador del juego
    GameController gameController;
    [SerializeField] GridStateController gridStateControllerBot;
    [SerializeField] GridStateController gridStateControllerPlayer;

    List<CardDetails> deck = new List<CardDetails>();
    public List<Transform> quadOnAttack = new List<Transform>();

    FireProjectile projectileSpawner;

    [SerializeField] GameObject cellCardPrefab;

    [SerializeField] GameObject cardContainer;

    public bool isChoosingCard = false;


    // Start is called before the first frame update
    void Start()
    {
        // Buscamos el controlador del juego para poder acceder a las cartas
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        gridStateControllerBot = GameObject.FindWithTag("EnemyGrid").GetComponent<GridStateController>();
        gridStateControllerPlayer = GameObject.FindWithTag("Grid").GetComponent<GridStateController>();
        projectileSpawner = GameObject.FindWithTag("EnemySpawnProjectile").GetComponent<FireProjectile>();
    }


    // Función para dar cartas al jugador en modo preparación
    public void GiveCardsInPreparationMode()
    {
        if (deck.Count < 5)
        {
            Cards cards = gameController.cards.DefenseCards();
            GiveCards(cards);
        }
    }

    // Función para alistar el estado de la partida del bot en Preparation Mode
    public void PreparationMode()
    {
        GiveCardsInPreparationMode();
        PlaceShips(true);
    }


    public IEnumerator ChooseCard(GameObject textCardChoosen){
        yield return new WaitForSeconds(2);
        Cards cards = gameController.cards;
        CardDetails card = cards.Items[Random.Range(0, cards.Items.Count)];
        GameObject cellCard = Instantiate(cellCardPrefab, cardContainer.transform);
        Transform cardObject = cellCard.transform.Find("Card");
        cardObject.localScale = new Vector3(2, 2, 2);
        Destroy(cardObject.GetComponent<CardController>());
        if(card.CardType == "Attack"){
            Attack(card);
            cardObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CartasAtaque/" + card.CardId.ToString());
        }else{
            cardObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/CartasDefensa/" + card.CardId.ToString());
            Defense(card);
        }
        textCardChoosen.SetActive(true);
        yield return new WaitForSeconds(3);
        textCardChoosen.SetActive(false);
        Destroy(cellCard);
    }
    
    // Método para dar cartas al jugador (animación de repartir cartas)
    public void GiveCards(Cards cards, int amountCarts = 5)
    {
        for (int i = 0; i < amountCarts; i++)
        {
            int randomIndex = Random.Range(0, cards.Items.Count);
            deck.Add(cards.Items[randomIndex]);
        }
    }


    // Método para poner barcos en el grid del bot
    public void PlaceShips(bool isPreparationMode = false, CardDetails randomCard = null)
    {
        List<CardDetails> deckToPlace = new List<CardDetails>();

        if (isPreparationMode)
            deckToPlace = deck;
        else
        {
            deckToPlace.Add(randomCard);
        }

        Transform grid = gridStateControllerBot.transform;

        foreach (CardDetails card in deckToPlace)
        {
            int randomX = Random.Range(0, 12);
            int randomY = Random.Range(0, 12);
            Transform startingQuad = grid.GetChild(randomX).GetChild(randomY);

            Debug.Log($"card coordinates: {card.LengthX} {card.LengthY}");
            Debug.Log($"startingQuad coordinates: {startingQuad.name}");

            if (gridStateControllerBot.ValidateShipPlacing(card, startingQuad)){
                gridStateControllerBot.PlaceShipMisile(card, startingQuad);
                Ship ship = new Ship();
                ship.Name = card.CardName;
                ship.LengthX = card.LengthX;
                ship.LengthY = card.LengthY;
                ship.quads = gridStateControllerBot.getQuadsList(ship.LengthX, ship.LengthY, startingQuad);
                gameController.enemyShips.Add(ship);
            }

        }



    }

    public void Defense(CardDetails card)
    {
        PlaceShips(randomCard: card);
    }

    public void Attack(CardDetails card)
    {
        // Cards cards = gameController.cards.AttackCards();
        // CardDetails card = cards.Items[Random.Range(0, cards.Items.Count)];

        Transform grid = gridStateControllerPlayer.transform;
        int randomX = Random.Range(0, 12);
        int randomY = Random.Range(0, 12);
        Transform startingQuad = grid.GetChild(randomX).GetChild(randomY);


        int currentX = int.Parse(startingQuad.name.Split(',')[0]);
        int currentY = int.Parse(startingQuad.name.Split(',')[1]);
        // El misil es horizontal
        if (card.LengthX > 1)
        {
            for (int i = currentX - card.LengthX; i < currentX; i++)
            {
                // ¿Está dentro del rango de grid?
                if (i >= 0 && i < startingQuad.parent.childCount)
                {
                    // Si sí, muestra efecto y sigue con el siguiente quad
                    Transform loopedQuad = startingQuad.parent.GetChild(i);
                    gameController.quadOnAttack.Add(loopedQuad);
                }
            }
            // El misil es vertical
        }
        else
        {
            for (int i = currentY; i < currentY + card.LengthY; i++)
            {
                // ¿Está dentro del rango de grid?
                if (i - 1 >= 0 && i - 1 < startingQuad.parent.parent.childCount && currentX - 1 >= 0 && currentX - 1 < startingQuad.parent.parent.GetChild(i - 1).childCount)
                {
                    // Si sí, muestra efecto y sigue con el siguiente quad
                    Transform loopedQuad = startingQuad.parent.parent.GetChild(i - 1).GetChild(currentX - 1);
                    gameController.quadOnAttack.Add(loopedQuad);
                }
            }
        }

        // LaunchProjectiles();
        gridStateControllerPlayer.PlaceShipMisile(card, startingQuad);
    }

    public IEnumerator LaunchProjectiles()
    {
        foreach (Transform quad in quadOnAttack)
        {
            projectileSpawner.LaunchProjectileBasedOnVelocity(quad);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1);
        quadOnAttack.Clear();
    }

}
