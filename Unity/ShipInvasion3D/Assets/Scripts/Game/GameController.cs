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

    BotCPU botCPU;

    Timer timer;



    // Variables para controlar si se está arrastrando una carta y
    // si se está usando una carta
    [HideInInspector] public bool isCardDragging = false;
    [HideInInspector] public bool isCardInUse = false;

    // Panel para jugar una carta (a donde se debe arrastrar para jugarla)
    [SerializeField] GameObject playCardPanel;

    // Panel para seleccionar una carta (mano del jugador en modo combate)
    public GameObject canvasCombat;
    // Panel para seleccionar una carta (mano del jugador en modo preparación)
    [SerializeField] GameObject canvasPreparation;

    // Variable para referenciar a la mano del jugador de la fase de preparación
    public GameObject playerHandPreparation;

    // Variable para contar las cartas en la mano del jugador
    public int cardsInHand = 0;

    // Referencia al botón para empezar el combate
    public Button startCombatButton;

    // Variable para autorizar si las cartas de defensa pueden poner barcos
    public bool ableToPlaceShip = false;

    // Variable para indicar si los quad deben hacer efecto de hover o no
    public bool quadHoverActive = false;

    // Enumerador para controlar el estado del juego
    [HideInInspector] public enum GameState {
        Main,
        AttackGrid,
        DefenseGrid,
        PCTurn,
        none
    }

    // Variable para controlar el estado actual del juego
    [HideInInspector] public  GameState currentState = GameState.none;

    // Referencia a todas las cartas disponibles en el juego
    public Cards cards;
    public PlayerDetails user;

    // Variable que guarda la longitud de una carta de ataque
    public int[] attackCardLength = new int[2];

    // Lista de quad a donde se mandarán proyectiles
    public List<Transform> quadOnAttack = new List<Transform>();

    // Variable para guardar el spawner de los proyectiles
    FireProjectile projectileSpawner;

    // Variable para indicar que se están lanzando misiles (para controlar que la lista no se limpie hasta que todos los misiles se hayan lanzado)
    // [HideInInspector] 
    public bool isLaunching = false;

    // Variable para indicar que el lanzamiento de misisles está activo (para controlar que en el loop no se ejecute muchas veces el lanzamiento en cada frame)
    public bool launchingActive = false;

    // Esta función se ejecutará al inicio del juego
    void Start()
    {
        // Obtenemos las referencias a los componentes necesarios
        // Controlador de la cámara para poderla mover dependiendo del estado del juego
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<MoveCamera>();
        // Script que reparte cartas al jugador 
        giveCards = GameObject.FindWithTag("CardsSpawner").GetComponent<GiveCards>();
        projectileSpawner = GameObject.FindWithTag("SpawnProjectile").GetComponent<FireProjectile>();
        botCPU = GameObject.FindWithTag("BotCPU").GetComponent<BotCPU>();
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();

        // Deserializamos las cartas disponibles en el juego (las cuardamos en una lista de cartas de manera que los datos estén disponibles en cualquier parte del juego)
        cards = JsonUtility.FromJson<Cards>(PlayerPrefs.GetString("cards"));
        user = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));

        Debug.Log(cards.Items[0].CardName);

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
            AttackMode();
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

        botCPU.PreparationMode();

        // Repartimos las cartas al jugador
        giveCards.GiveCardsInPreparationMode();

        // TODO: Choose and "place the ships" of the AI

    }

    // Función que se ejecuta en el estado principal
    void HandleMainState(){
        // Debug.Log("Main State");
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        // Mostar el canvas de combate por si no se está mostrando
        canvasCombat.SetActive(true);
        // Ocultar el canvas de preparación/defenseMode
        canvasPreparation.SetActive(false);
        // Deshabilitar que se puedan instanciar barcos para las cartas de defensa
        ableToPlaceShip = false;
        quadHoverActive = false;
        isCameraOnAttack = false;
        isCameraOnDefense = false;
        // Movemos los grids para que no sean visibles
        StartCoroutine(MoveGrid(false, 0.5f));
        StartCoroutine(MoveGridEnemy(false, 0.5f));
        // Dar cartas al jugador
        // Si tiene 0 cartas, se reparten 5 cartas (al inicio del juego)
        if(cardsInHand == 0){
            giveCards.GiveCardsInCombatMode();
        // Si tiene más de 0 cartas y menos de 5 (normalmente tendría 4, porque acaba de usar una), 
        // entonces se reparte sólo una carta
        }else if (cardsInHand > 0 && cardsInHand < 5){
            giveCards.GiveCardInCombatMode();
        }
        if(quadOnAttack.Count > 0 && !launchingActive){
            StartCoroutine(LaunchProjectiles());
            launchingActive = true;
        }
    }



    // Función para activar el modo de ataque
    public void AttackMode(){
        // Debug.Log("AttackGrid State");
        // Si la cámara no está en modo ataque, se pone en modo ataque
        if (!isCameraOnAttack){
            quadHoverActive = true;
            isCameraOnAttack = true;
            // Movemos la cámara a la posición de ataque
            cameraController.MoveCameraToAttack();
            // Movemos el grid del enemigo para que sea visible
            StartCoroutine(MoveGridEnemy(true, 0.5f));
            // Desactivamos el panel de selección de cartas (la mano del jugador)
            canvasCombat.SetActive(false);
        }
    }

    // Función para activar el modo de defensa
    public void DefenseMode(){
        // Debug.Log("DefenseGrid State");
        // Si la cámara no está en modo defensa, se pone en modo defensa
        if (!isCameraOnDefense){
            quadHoverActive = true;
            isCameraOnDefense = true;
            // Habilitar que se instancien barcos para las cartas de defensa
            ableToPlaceShip = true;
            // Mover el contenedor de la mano del jugador a la parte lateral de la pantalla
            RectTransform rectTransformPlayerHand = playerHandPreparation.GetComponent<RectTransform>();
            rectTransformPlayerHand.sizeDelta = new Vector2(300, 300);
            rectTransformPlayerHand.anchoredPosition = new Vector2(-800, 700);
            // Reducir su espaciado para que las cartas se encimen y quepan en la pantalla
            GridLayoutGroup gridLayout = playerHandPreparation.GetComponent<GridLayoutGroup>();
            gridLayout.spacing =  new Vector2(gridLayout.spacing.x, -100);
            canvasPreparation.SetActive(true);
            startCombatButton.gameObject.SetActive(false);
            // Movemos la cámara a la posición de defensa
            cameraController.MoveCameraToDefense();
            // Movemos el grid del jugador para que sea visible
            StartCoroutine(MoveGrid(true, 0.5f)); 
            // Desactivamos el panel de selección de cartas (la mano del jugador)
            canvasCombat.SetActive(false);

            botCPU.Attack();
            botCPU.Defense();
        }
    }

    // Función para cambiar el modo de preparación a modo de combate
    public void StartCombatMode(){
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        // Ocultar canvas de la fase de preparación
        canvasPreparation.SetActive(false);
        // Mostrar canvas de la fase de combate
        canvasCombat.SetActive(true);
        // Cambiar estado a main para empezar fase de combate
        currentState = GameState.Main;

        timer.StartTimer(timer.matchTime);
    }

    // Function to set the attack state of the game
    public void SetAttackGridState(){
        currentState = GameState.AttackGrid;
    }

    // Function to set the attack state of the game
    public void SetMainState(){
        currentState = GameState.Main;
    }

    // Function to set the defense state of the game
    public void SetDefenseGridState(){
        currentState = GameState.DefenseGrid;
    }

    public void SetPcTurnGridState(){
        currentState = GameState.PCTurn;
    }

    // Corrutina para mover el grid del enemigo  que recibe la duración de la animación
    IEnumerator MoveGridEnemy(bool showGrid, float duration = 1.0f){
        // Posición inicial y final del grid del enemigo
        Vector3 start = enemyGrid.position;
        // Obtener el punto a donde se va a mover el grid del enemigo (1.3 hacia arriba si la cámara está en modo ataque, 1.3 hacia abajo si no lo está)
        Vector3 target = new Vector3(enemyGrid.position.x, showGrid ? 20.1f : 18.8f, enemyGrid.position.z);
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
    IEnumerator MoveGrid(bool showGrid, float duration = 1.0f){
        // Posición inicial y final del grid del jugador
        Vector3 start = grid.position;
        // Obtener el punto a donde se va a mover el grid del jugador (1.3 hacia arriba si la cámara está en modo defensa, 1.3 hacia abajo si no lo está)
        Vector3 target = new Vector3(grid.position.x, showGrid ? 20.1f : 18.8f, grid.position.z);
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

    public IEnumerator LaunchProjectiles(){
        foreach(Transform quad in quadOnAttack){
            projectileSpawner.LaunchProjectileBasedOnVelocity(quad);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1);
        quadOnAttack.Clear();
        isLaunching = false;
        launchingActive = false;
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
