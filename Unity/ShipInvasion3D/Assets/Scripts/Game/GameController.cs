using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Este script sirve para controlar el flujo del juego y las diferentes fases del juego


public class GameController : MonoBehaviour
{   
    // Referencia a los grids de ataque y defensa
    [SerializeField] Transform enemyGrid;
    [SerializeField] Transform grid;

    // Variables para controlar si la cámara está en modo ataque o defensa
    private bool isCameraOnAttack = false;
    private bool isCameraOnDefense = false;

    // Referencia al controlador de la cámara
    MoveCamera cameraController;

    // Referencia al script que reparte cartas al jugador
    GiveCards giveCards;

    // Variables para controlar si se está arrastrando una carta y
    // si se está usando una carta
    [HideInInspector] public bool isCardDragging = false;
    [HideInInspector] public bool isCardInUse = false;

    // Panel para jugar una carta (a donde se debe arrastrar para jugarla)
    [SerializeField] GameObject playCardPanel;

    // Panel para seleccionar una carta (mano del jugador)
    [SerializeField] GameObject canvasSelectCard;

    // Variable para referenciar a la mano del jugador de la fase de preparación
    public GameObject playerHandPreparation;

    // Variable para contar las cartas en la mano del jugador
    public int cardsInHand = 0;

    // Referencia al botón para empezar el combate
    public Button startCombatButton;

    // Enumerador para controlar el estado del juego
    enum GameState {
        Main,
        AttackGrid,
        DefenseGrid,
        PCTurn
    }

    // Variable para controlar el estado actual del juego
    private GameState currentState;

    // Referencia a las todas las cartas disponibles en el juego
    public Cards cards;

    // Esta función se ejecutará al inicio del juego
    void Start()
    {
        // Obtenemos las referencias a los componentes necesarios
        // Controlador de la cámara para poderla mover dependiendo del estado del juego
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<MoveCamera>();
        // Script que reparte cartas al jugador 
        giveCards = GameObject.FindWithTag("CardsSpawner").GetComponent<GiveCards>();
        // Deserializamos las cartas disponibles en el juego (las cuardamos en una lista de cartas de manera que los datos estén disponibles en cualquier parte del juego)
        cards = JsonUtility.FromJson<Cards>(PlayerPrefs.GetString("cards"));
        // Inicializamos el estado del juego en el estado principal (fase de preparación)
        StartCoroutine(PreparationMode());
    }

    // Esta función se ejecutará en cada frame
    void Update()
    {
        // Máquina de estados para controlar el flujo del juego
        if(currentState == GameState.Main){
            // Si estamos en el estado principal, repartimos las cartas al jugador o esperamos a que utilice alguna carta
            HandleMainState();
        }else if(currentState == GameState.AttackGrid){
            // Si estamos en el estado de ataque, activamos el modo de ataque y esperamos a que el jugador seleccione a donde va a atacar
            AtackMode();
        }else if(currentState == GameState.DefenseGrid){
            // Si estamos en el estado de defensa, activamos el modo de defensa y esperamos a que el jugador seleccione a donde va a defender (poner barco)
            DefenseMode();
        }else if(currentState == GameState.PCTurn){
            // Si estamos en el estado de turno de la PC, activamos el modo de ataque o defensa de la PC
            // PCPlay();
        }

    }

    // Corrutina para la fase de preparación
    IEnumerator PreparationMode(){
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        // Desactivamos los modos de ataque y defensa
        isCameraOnAttack = false;
        isCameraOnDefense = false;
        
        // Esperamos un poco antes de repartir las cartas
        yield return new WaitForSeconds(0.2f);

        // Repartimos las cartas al jugador
        giveCards.GiveCardsInPreparationMode();

        // TODO: **Estas dos siguientes líneas describen lo que se debe hacer**
        // 1. Esperamos a que el jugador coloque todos sus barcos iniciales
        // 2. Cambiamos el estado del juego al estado principal
    }

    // Función que se ejecuta en el estado principal
    void HandleMainState(){
        // Dar cartas al jugador
        giveCards.GiveCardsInCombatMode();

        // TODO: **Estas dos siguientes líneas describen lo que se debe hacer**
        // 1. Esperamos a que el jugador utilice una carta
        // 2. Cambiar al estado de ataque o defensa dependiendo del tipo de carta
    }



    // Función para activar el modo de ataque
    public void AtackMode(){
        // Si la cámara no está en modo ataque, se pone en modo ataque
        if (!isCameraOnAttack){
            isCameraOnAttack = true;
            // Movemos la cámara a la posición de ataque
            cameraController.MoveCameraToAttack();
            // Movemos el grid del enemigo para que sea visible
            StartCoroutine(MoveGridEnemy());
            // Desactivamos el panel de selección de cartas (la mano del jugador)
            canvasSelectCard.SetActive(false);
        }
    }

    // Función para activar el modo de defensa
    public void DefenseMode(){
        // Si la cámara no está en modo defensa, se pone en modo defensa
        if (!isCameraOnDefense){
            isCameraOnDefense = true;
            // Movemos la cámara a la posición de defensa
            cameraController.MoveCameraToDefense();
            // Movemos el grid del jugador para que sea visible
            StartCoroutine(MoveGrid());
            // Desactivamos el panel de selección de cartas (la mano del jugador)
            canvasSelectCard.SetActive(false);
        }
    }

    // Corrutina para mover el grid del enemigo  que recibe la duración de la animación
    IEnumerator MoveGridEnemy(float duration = 1.0f){
        // Posición inicial y final del grid del enemigo
        Vector3 start = enemyGrid.position;
        // Obtener el punto a donde se va a mover el grid del enemigo (1.3 hacia arriba si la cámara está en modo ataque, 1.3 hacia abajo si no lo está)
        Vector3 target = new Vector3(enemyGrid.position.x, isCameraOnAttack ? enemyGrid.position.y + 1.3f : enemyGrid.position.y - 1.3f, enemyGrid.position.z);
        float time = 0;
        // Mientras no haya pasado el tiempo de duración, se mueve el grid del enemigo
        while (time < duration) {
            // Mueve el grid del jugador poco a poco hasta llegar a la posición destino (para que se vea un efecto de animación)
            enemyGrid.position = Vector3.Lerp(start, target, time / duration);
            // Aumenta el tiempo transcurrido
            time += Time.deltaTime;
            yield return null; // Espera hasta el próximo frame
        }
        enemyGrid.position = target; // Asegura que el objeto llegue a la posición destino
    }


    // Corrutina para mover el grid del jugador que recibe la duración de la animación
    IEnumerator MoveGrid(float duration = 1.0f){
        // Posición inicial y final del grid del jugador
        Vector3 start = grid.position;
        // Obtener el punto a donde se va a mover el grid del jugador (1.3 hacia arriba si la cámara está en modo defensa, 1.3 hacia abajo si no lo está)
        Vector3 target = new Vector3(grid.position.x, isCameraOnDefense ? grid.position.y + 1.3f : grid.position.y - 1.3f, grid.position.z);
        float time = 0; 
        // Mientras no haya pasado el tiempo de duración, se mueve el grid del jugador
        while (time < duration) {
            // Mueve el grid del jugador poco a poco hasta llegar a la posición destino (para que se vea un efecto de animación)
            grid.position = Vector3.Lerp(start, target, time / duration);
            // Aumenta el tiempo transcurrido
            time += Time.deltaTime;
            yield return null; // Espera hasta el próximo frame
        }
        grid.position = target; // Asegura que el objeto llegue a la posición destino
    }

    // Función para avisar que se está arrastrando una carta
    public void OnCardDrag(){
        isCardDragging = true;
        playCardPanel.SetActive(true);
    }

    // Función para avisar que se soltó una carta
    public void OnCardDrop(){
        isCardDragging = false;
        if (!isCardInUse){
            playCardPanel.SetActive(false);
        }
    }
}
