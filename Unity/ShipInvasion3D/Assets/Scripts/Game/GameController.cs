using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{   
    [SerializeField] Transform enemyGrid;
    [SerializeField] Transform grid;
    private bool isCameraOnAttack = false;
    private bool isCameraOnDefense = false;
    MoveCamera cameraController;
    GiveCards giveCards;
    [HideInInspector] public bool isCardDragging = false;
    [HideInInspector] public bool isCardInUse = false;
    [HideInInspector] public int CardsCounter = 0;

    [SerializeField] GameObject playCardPanel;
    [SerializeField] GameObject canvasSelectCard;

    enum GameState {
        Main,
        AttackGrid,
        DefenseGrid,
        PCTurn
    }

    private GameState currentState = GameState.Main;

    public Cards cards;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<MoveCamera>();
        giveCards = GameObject.FindWithTag("CardsSpawner").GetComponent<GiveCards>();
        cards = JsonUtility.FromJson<Cards>(PlayerPrefs.GetString("cards"));
        PreparationMode();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.Main){
            // Debug.Log(CardsCounter);
            // if(CardsCounter < 5){
            //     HandleMainState();
            // }
        }else if(currentState == GameState.AttackGrid){
            AtackMode();
        }else if(currentState == GameState.DefenseGrid){
            DefenseMode();
        }else if(currentState == GameState.PCTurn){
            // PCPlay();
        }

    }

    void PreparationMode(){
        cameraController.MoveCameraToOrigin();
        isCameraOnAttack = false;
        isCameraOnDefense = false;
        // StartCoroutine(MoveGrid());
    }

    void HandleMainState(){
        giveCards.GiveCardsInCombatMode();
    }









    public void AtackMode(){
        if (!isCameraOnAttack){
            cameraController.MoveCameraToAttack();
            isCameraOnAttack = true;
            StartCoroutine(MoveGridEnemy());
            canvasSelectCard.SetActive(false);
        }
    }

    public void DefenseMode(){
        if (!isCameraOnDefense){
            cameraController.MoveCameraToDefense();
            isCameraOnDefense = true;
            StartCoroutine(MoveGrid());
            canvasSelectCard.SetActive(false);
        }
    }

    IEnumerator MoveGridEnemy(float duration = 1.0f){
        Vector3 start = enemyGrid.position;
        Vector3 target = new Vector3(enemyGrid.position.x, isCameraOnAttack ? enemyGrid.position.y + 1.3f : enemyGrid.position.y - 1.3f, enemyGrid.position.z);
        float time = 0;

        while (time < duration) {
            enemyGrid.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null; // Espera hasta el próximo frame
        }

        enemyGrid.position = target; // Asegura que el objeto llegue a la posición destino
    }

        IEnumerator MoveGrid(float duration = 1.0f){
        Vector3 start = grid.position;
        Vector3 target = new Vector3(grid.position.x, isCameraOnDefense ? grid.position.y + 1.3f : grid.position.y - 1.3f, grid.position.z);
        float time = 0;

        while (time < duration) {
            grid.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null; // Espera hasta el próximo frame
        }

        grid.position = target; // Asegura que el objeto llegue a la posición destino
    }

    public void OnCardDrag(){
        isCardDragging = true;
        playCardPanel.SetActive(true);
    }

    public void OnCardDrop(){
        isCardDragging = false;
        if (!isCardInUse){
            playCardPanel.SetActive(false);
        }
    }
}
