using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Este script sirve para controlar el flujo del juego y las diferentes fases del juego


public class GameController : MonoBehaviour
{   
    // Referencia a los grids de ataque y defensa
    [SerializeField] Transform enemyGrid;
    [SerializeField] Transform grid;

    // Variables para controlar si la cámara está en modo ataque o defensa
    private bool isCameraOnAttack = false;
    private bool isCameraOnDefense = false;

    public bool isPreparationMode = true;

    // Referencia al controlador de la cámara
    MoveCamera cameraController;

    // Referencia al script que reparte cartas al jugador
    GiveCards giveCards;

    BotCPU botCPU;
    Arena arena;

    Timer timer;

    public APIConnection API = null;

    public GameIdClass gameIdClass;

    public CardDetails currentAttackCard;

    public int playNumber = 1;

    public List<Ship> ships;
    public List<Ship> enemyShips;

    [SerializeField] AudioSource audioPreparation;
    [SerializeField] AudioSource audioCombat;
    public AudioSource audioDefenseCard;
    [SerializeField] AudioSource audioStartCombat;

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
    // Panel para anunciar que es turno de la PC y mostar su carta elegida
    [SerializeField] GameObject canvasBot;

    // Variable para referenciar a la mano del jugador de la fase de preparación
    public GameObject playerHandPreparation;

    // Variable para contar las cartas en la mano del jugador
    public int cardsInHand = 0;

    // Referencia al botón para empezar el combate
    public Button startCombatButton;
    public GameObject preparationModeIndicator;

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

    // Variable para guardar los spawners de los proyectiles
    FireProjectile projectileSpawner;
    FireProjectile projectileEnemySpawner;

    // Variable para indicar que se están lanzando misiles (para controlar que la lista no se limpie hasta que todos los misiles se hayan lanzado)
    // [HideInInspector] 
    public bool isLaunching = false;

    // Variable para indicar que el lanzamiento de misisles está activo (para controlar que en el loop no se ejecute muchas veces el lanzamiento en cada frame)
    public bool launchingActive = false;

    public bool showQuadsAttacked = false;

    public int quadsAttacked = 0;

    public SceneConnection sceneConnection = null;
    bool resetTimerPosition = true;

    Arena arenaLoaded;

    [SerializeField] List<Material> waterMaterials;

    [SerializeField] MeshRenderer water;

    [SerializeField] List<Material> skyboxMaterials;

    // Esta función se ejecutará al inicio del juego
    void Start()
    {
        arenaLoaded = JsonUtility.FromJson<Arena>(PlayerPrefs.GetString("gameArena"));
        SetearArena(arenaLoaded.Id);
        audioPreparation.Play();
        // Obtenemos las referencias a los componentes necesarios
        // Controlador de la cámara para poderla mover dependiendo del estado del juego
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<MoveCamera>();
        // Script que reparte cartas al jugador 
        giveCards = GameObject.FindWithTag("CardsSpawner").GetComponent<GiveCards>();
        // Script que manda los misisles del jugador hacia la PC
        projectileSpawner = GameObject.FindWithTag("SpawnProjectile").GetComponent<FireProjectile>();
        projectileEnemySpawner = GameObject.FindWithTag("EnemySpawnProjectile").GetComponent<FireProjectile>();
        // Script que manda los misisles de la PC hacia el jugador
        botCPU = GameObject.FindWithTag("BotCPU").GetComponent<BotCPU>();
        // Script que controlla el timer del juego
        timer = GameObject.FindWithTag("Timer").GetComponent<Timer>();
        // Script que sirve para cargar otras escenas
        sceneConnection = GameObject.FindWithTag("SceneConnection").GetComponent<SceneConnection>();

        // Deserializamos las cartas disponibles en el juego (las cuardamos en una lista de cartas de manera que los datos estén disponibles en cualquier parte del juego)
        cards = JsonUtility.FromJson<Cards>(PlayerPrefs.GetString("cards")).SortCards();
        cards.BalanceCards();
        user = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));
        API = GameObject.FindWithTag("APIConnection").GetComponent<APIConnection>();
        if (API == null) {
            Debug.LogError("APIConnection component not found on the object.");
        }

        arena = JsonUtility.FromJson<Arena>(PlayerPrefs.GetString("gameArena"));
        Debug.Log(PlayerPrefs.GetString("gameArena"));
        if (arena == null)
            Debug.LogError("Failed to deserialize arena data.");

        // mandar 
        StartCoroutine(API.PutGame("0", user?.PlayerId.ToString(), arena.Id.ToString()));

        // Inicializamos el estado del juego en el estado principal (fase de preparación)
        StartCoroutine(PreparationMode());
    }

    // Esta función se ejecutará en cada frame
    void Update()
    {
        // Máquina de estados para controlar el flujo del juego
        if(currentState == GameState.Main){
            // Si estamos en el estado principal, repartimos las cartas al jugador o esperamos a que utilice alguna carta
            StartCoroutine(HandleMainState());
        }else if(currentState == GameState.AttackGrid){
            // Si estamos en el estado de ataque, activamos el modo de ataque y esperamos a que el jugador seleccione a donde va a atacar
            AttackMode();
        }else if(currentState == GameState.DefenseGrid){
            // Si estamos en el estado de defensa, activamos el modo de defensa y esperamos a que el jugador seleccione a donde va a defender (poner barco)
            DefenseMode();
        }else if(currentState == GameState.PCTurn){
            // Si estamos en el estado de turno de la PC, activamos el modo de ataque o defensa de la PC
            if(!botCPU.isChoosingCard){
                StartCoroutine(PCPlay());
            }
        }

    }

    IEnumerator PCPlay(){
        botCPU.isChoosingCard = true;
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        // Mostar el canvas de combate por si no se está mostrando
        canvasCombat.SetActive(false);
        // Ocultar el canvas de preparación/defenseMode
        canvasPreparation.SetActive(false);
        // Movemos los grids para que no sean visibles
        StartCoroutine(MoveGrid(false, 0.5f));
        StartCoroutine(MoveGridEnemy(false, 0.5f));
        // Deshabilitar que se puedan instanciar barcos para las cartas de defensa
        ableToPlaceShip = false;
        quadHoverActive = false;
        isCameraOnAttack = false;
        isCameraOnDefense = false;
        if(quadOnAttack.Count > 0 && !launchingActive){
            launchingActive = true;
            yield return StartCoroutine(LaunchProjectiles());
        }
        // Mostrar el panel que anuncia que es turno de la PC
        canvasBot.SetActive(true);
        canvasBot.transform.Find("Quad Attacked").gameObject.SetActive(true);
        canvasBot.transform.Find("Quad Attacked").GetComponent<TMP_Text>().text = $"{quadsAttacked} Celdas Atacadas";
        yield return new WaitForSeconds(1);
        canvasBot.transform.Find("Quad Attacked").gameObject.SetActive(false);
        quadsAttacked = 0;
        canvasBot.transform.Find("Barco Caído").gameObject.SetActive(false);
        canvasBot.transform.Find("Titulo").gameObject.SetActive(true);
        StartCoroutine(ValidateEndGame());
        yield return StartCoroutine(botCPU.ChooseCard(canvasBot.transform.Find("Carta Utilizada Texto").gameObject));
        currentState = GameState.Main;
        botCPU.isChoosingCard = false;
    }

    // Función que se ejecuta en el estado principal
    IEnumerator HandleMainState(){
        // Debug.Log("Main State");
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        if(!isCardDragging){
            playCardPanel.SetActive(false);
        }
        // Ocultar el canvas de preparación/defenseMode
        canvasPreparation.SetActive(false);
        // Ocultar el panel que anuncia que es turno de la PC
        canvasBot.SetActive(false);
        // Movemos los grids para que no sean visibles
        StartCoroutine(MoveGrid(false, 0.5f));
        StartCoroutine(MoveGridEnemy(false, 0.5f));
        // Deshabilitar que se puedan instanciar barcos para las cartas de defensa
        ableToPlaceShip = false;
        quadHoverActive = false;
        isCameraOnAttack = false;
        isCameraOnDefense = false;
        if(quadOnAttack.Count > 0 && !launchingActive){
            launchingActive = true;
            yield return StartCoroutine(LaunchProjectiles(true));
            showQuadsAttacked = true;
        }
        if(showQuadsAttacked){
            canvasCombat.transform.Find("Quad Attacked").gameObject.SetActive(true);
            canvasCombat.transform.Find("Quad Attacked").GetComponent<TMP_Text>().text = $"{quadsAttacked} Celdas Atacadas";
            yield return new WaitForSeconds(1);
            canvasCombat.transform.Find("Barco Caído").gameObject.SetActive(false);
            canvasCombat.transform.Find("Quad Attacked").gameObject.SetActive(false);
            quadsAttacked = 0;
            showQuadsAttacked = false;
        }
        // Mostar el canvas de combate para que el jugador pueda seleccionar una carta
        canvasCombat.SetActive(true);
        canvasCombat.transform.Find("Ships Couter").gameObject.SetActive(true);
        canvasCombat.transform.Find("Attack Indicator").gameObject.SetActive(false);
        canvasCombat.transform.Find("Defense Indicator").gameObject.SetActive(false);
        if(!resetTimerPosition){
            Transform timerTransform = canvasCombat.transform.Find("MatchTime");
            canvasCombat.transform.Find("MatchTime").localPosition = new Vector3(0, 510, 0);
            resetTimerPosition = true;
        }
        StartCoroutine(ValidateEndGame());
        // Dar cartas al jugador
        // Si tiene 0 cartas, se reparten 5 cartas (al inicio del juego)
        if(cardsInHand == 0){
            giveCards.GiveCardsInCombatMode();
        // Si tiene más de 0 cartas y menos de 5 (normalmente tendría 4, porque acaba de usar una), 
        // entonces se reparte sólo una carta
        }else if (cardsInHand > 0 && cardsInHand < 5){
            giveCards.GiveCardInCombatMode();
        }
        yield return null;
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

        yield return new WaitForSeconds(2f);
        gameIdClass = JsonUtility.FromJson<GameIdClass>(PlayerPrefs.GetString("gameId"));
        Debug.Log($"Game Id: {gameIdClass.GameId}");
        botCPU.PreparationMode();
        yield return new WaitForSeconds(2f);
        canvasPreparation.transform.Find("Mensaje de Preparacion").gameObject.SetActive(false);
    }


    // Función para activar el modo de ataque
    public void AttackMode(){
        // Debug.Log("AttackGrid State");
        // Si la cámara no está en modo ataque, se pone en modo ataque
        if (!isCameraOnAttack){
            quadHoverActive = true;
            isCameraOnAttack = true;
            // Movemos el grid del enemigo para que sea visible
            StartCoroutine(MoveGridEnemy(true, 0.5f));
            // Movemos la cámara a la posicion de ataque
            cameraController.MoveCameraToAttack();


            // Desactivamos el panel de selección de cartas (la mano del jugador) exepcto el timer y el indicador de modo
            // canvasCombat.SetActive(false);
            canvasCombat.transform.Find("Cards").gameObject.SetActive(false);
            canvasCombat.transform.Find("Attack Indicator").gameObject.SetActive(true);
            canvasCombat.transform.Find("Defense Indicator").gameObject.SetActive(false);
            canvasCombat.transform.Find("UseCardPanel").gameObject.SetActive(false);
            if(resetTimerPosition){
                Transform timerTransform = canvasCombat.transform.Find("MatchTime");
                canvasCombat.transform.Find("MatchTime").localPosition = new Vector3(734, 350, 0);
                resetTimerPosition = false;
            }
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
            // Movemos el grid del jugador para que sea visible
            StartCoroutine(MoveGrid(true, 0.5f));
            // Mover el contenedor de la mano del jugador a la parte lateral de la pantalla
            RectTransform rectTransformPlayerHand = playerHandPreparation.GetComponent<RectTransform>();
            rectTransformPlayerHand.sizeDelta = new Vector2(400, 950);
            rectTransformPlayerHand.anchoredPosition = new Vector2(-800, -60);
            // Reducir su espaciado para que las cartas se encimen y quepan en la pantalla
            GridLayoutGroup gridLayout = playerHandPreparation.GetComponent<GridLayoutGroup>();
            gridLayout.spacing =  new Vector2(gridLayout.spacing.x, -150);
            canvasPreparation.SetActive(true);
            startCombatButton.gameObject.SetActive(false);
            preparationModeIndicator.SetActive(false);
            // Movemos la cámara a la posición de defensa
            cameraController.MoveCameraToDefense();


            // Desactivamos el panel de selección de cartas (la mano del jugador) exepcto el timer y el indicador de modo
            // canvasCombat.SetActive(false);
            canvasCombat.transform.Find("Cards").gameObject.SetActive(false);
            canvasCombat.transform.Find("Ships Couter").gameObject.SetActive(false);
            canvasCombat.transform.Find("Attack Indicator").gameObject.SetActive(false);
            canvasCombat.transform.Find("Defense Indicator").gameObject.SetActive(true);
            canvasCombat.transform.Find("UseCardPanel").gameObject.SetActive(false);
            if(!isPreparationMode && resetTimerPosition){
                Transform timerTransform = canvasCombat.transform.Find("MatchTime");
                canvasCombat.transform.Find("MatchTime").localPosition = new Vector3(734, 350, 0);
                resetTimerPosition = false;
            }

        }
    }

    // Función para cambiar el modo de preparación a modo de combate
    public void StartCombatMode(){

        isPreparationMode = false;
        // Movemos la cámara a la posición original
        cameraController.MoveCameraToOrigin();
        // Ocultar canvas de la fase de preparación
        canvasPreparation.SetActive(false);
        // Mostrar canvas de la fase de combate
        canvasCombat.SetActive(true);
        canvasCombat.transform.Find("Cards").gameObject.SetActive(true);
        // Cambiar estado a main para empezar fase de combate
        currentState = GameState.Main;
        audioPreparation.Stop();
        audioStartCombat.Play();
        audioCombat.Play();

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

    public IEnumerator LaunchProjectiles(bool enemy = false){
        foreach(Transform quad in quadOnAttack){
            if(enemy){
                projectileEnemySpawner.LaunchProjectileBasedOnVelocity(quad);
            }else{
                projectileSpawner.LaunchProjectileBasedOnVelocity(quad);
            }

            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1);
        quadOnAttack.Clear();
        isLaunching = false;
        launchingActive = false;
        if(currentState == GameState.Main){
            yield return new WaitForSeconds(1);yield return new WaitForSeconds(1);
            canvasCombat.transform.Find("Cards").gameObject.SetActive(true);
        }
    }

    // Función para avisar que se está arrastrando una carta
    public void OnCardDrag(){
        isCardDragging = true;
        if(currentState == GameState.Main){
            playCardPanel.SetActive(true);
        }
    }

    // Función para avisar que se soltó una carta
    public void OnCardDrop(){
        isCardDragging = false;
        if (!isCardInUse){
            playCardPanel.SetActive(false);
        }
    }

    public void CheckSunkenShip(bool pcShips = false){
        List<Ship> shipList = pcShips ? enemyShips : ships;
        foreach(Ship ship in shipList){
            bool sunken = false;
            foreach(Transform quad in ship.quads){
                if(quad.GetComponent<Quad>().state == Quad.quadState.hit){
                    sunken = true;
                }else{
                    sunken = false;
                    break;
                }
            }
            if(sunken){
                ship.sunken = true;
            }else{
                ship.sunken = false;
            }
        }
    }

    public IEnumerator ValidateEndGame(){
        int activeShips = 0;
        int sunkenShips = 0;
        int activeEnemyShips = 0;
        int sunkenEnemyShips = 0;

        int damageByUser = 0;
        int damageByEnemy = 0;

        foreach(Ship ship in enemyShips){
            if(ship.sunken)
            {
                sunkenEnemyShips++;
                damageByUser = damageByUser + ship.quads.Count;
            }
            else
            {
                activeEnemyShips++;
                for (int i = 0; i < ship.quads.Count; i++)
                {
                    if (ship.quads[i].GetComponent<Quad>().state == Quad.quadState.hit)
                        damageByUser++;
                }
            }
        }
        foreach(Ship ship in ships){
            if(ship.sunken)
            {
                sunkenShips++;
                damageByEnemy = damageByEnemy + ship.quads.Count;
            }
            else
            {
                activeShips++;
                for (int i = 0; i < ship.quads.Count; i++)
                {
                    if (ship.quads[i].GetComponent<Quad>().state == Quad.quadState.hit)
                        damageByEnemy++;
                }
            }
        }

        if (sunkenShips == ships.Count || sunkenEnemyShips == enemyShips.Count || timer.isTimerActive == false)
        {
            Debug.Log(activeShips);
            Debug.Log(sunkenShips);
            Debug.Log(activeEnemyShips);
            Debug.Log(sunkenEnemyShips);
            Debug.Log(damageByUser);
            Debug.Log(damageByEnemy);

            PlayerPrefs.SetInt("activeShips", activeShips);
            PlayerPrefs.SetInt("sunkenShips", sunkenShips);
            PlayerPrefs.SetInt("activeEnemyShips", activeEnemyShips);
            PlayerPrefs.SetInt("sunkenEnemyShips", sunkenEnemyShips);
            PlayerPrefs.SetInt("damageByUser", damageByUser);
            PlayerPrefs.SetInt("damageByEnemy", damageByEnemy);
            yield return new WaitForSeconds(1);
            // Cambiar de escena
            sceneConnection.toEndGame();
        }
    }

    void SetearArena(int id){
        if(id == 1){
            Debug.Log("Mar Abierto");
            water.materials = new Material[] { waterMaterials[0] };
            RenderSettings.skybox = skyboxMaterials[0];
            DynamicGI.UpdateEnvironment();
        }else if(id == 2){
            Debug.Log("Tormenta Eléctrica");
            water.materials = new Material[] { waterMaterials[1] };
            RenderSettings.skybox = skyboxMaterials[1];
            DynamicGI.UpdateEnvironment();
        }else if(id == 3){
            Debug.Log("Río de Fuego");
            water.materials = new Material[] { waterMaterials[2] };
            RenderSettings.skybox = skyboxMaterials[2];
            DynamicGI.UpdateEnvironment();
        }else if(id == 4){
            Debug.Log("Pantano tóxico");
            water.materials = new Material[] { waterMaterials[3] };
            RenderSettings.skybox = skyboxMaterials[3];
            DynamicGI.UpdateEnvironment();
        }else{
            Debug.LogError("Error al cargar arena");
        }
    }
}
