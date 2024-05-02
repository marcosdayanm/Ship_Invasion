using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// Esta clase controla el comportamiento de las cartas como el drag and drop, hover, 
// mostrar un barco cuando es de tipo Defense, etc.

public class CardController :  
                                MonoBehaviour, // Necesario para que el script pueda ser un componente
                                IPointerEnterHandler,  // Necesario para detectar el puntero entrando en el objeto
                                IPointerExitHandler, // Necesario para detectar el puntero saliendo del objeto
                                IBeginDragHandler,  // Necesario para detectar el inicio de un arrastre
                                IDragHandler,  // Necesario para detectar el arrastre
                                IEndDragHandler // Necesario para detectar el fin de un arrastre
{
    private bool isDragging = false;
    private bool isHovering = false;

    // Esta variable guarda la referencia al padre original de cada carta (para que cuando sea necesario pueda volver a dicho contenedor)
    [HideInInspector] public Transform parentToReturnTo = null;

    // Prefab del barco que se instanciará al arrastrar una carta de tipo Defense
    [SerializeField] public GameObject shipPrefab;

    // Referencia al GameController para acceder a sus métodos y propiedades
    GameController gameController;

    // Referencia al GridStateController para acceder a sus métodos y propiedades
    [SerializeField] GridStateController gridStateController;

    // Referencia a la imagen de la carta (para desactivar el raycastTarget mientras se arrastra)
    [SerializeField] public Image image;

    // Variable para almacenar la información de una sola carta (nombre, tipo, tamaño de barco, etc.)
    // [HideInInspector] 
    public CardDetails cardDetails;

    // Referencia al objeto que se instanciará al arrastrar una carta de tipo Defense
    private GameObject currentShipInstance;

    // Variables para ajustar la posición y rotación del barco en el tablero
    private Vector3 fixPosition;
    private Quaternion fixRotation;

    Game game;
    

    // Start is called before the first frame update
    void Start()
    {
        // Obtenemos la referencia al GameController
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        gridStateController = GameObject.FindWithTag("Grid").GetComponent<GridStateController>();

        game = JsonUtility.FromJson<Game>(PlayerPrefs.GetString("game"));
    }


    // Esta función se ejecutará cuando se hace hover sobre una carta
    public void OnPointerEnter(PointerEventData eventData){
        // Si no hay ninguna carta en uso y no se está arrastrando ninguna carta
        if(!gameController.isCardInUse){
            // Si no se está arrastrando la carta
            if(!isDragging){
                // Aumentamos el tamaño de la carta y la movemos hacia arriba para que se vea mejor
                transform.position = new Vector3(transform.position.x, transform.position.y + 90, transform.position.z);
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                // Guardamos la referencia al padre original de la carta
                parentToReturnTo = transform.parent;
                // Cambiamos el padre de la carta al root para que no se vea afectada por otros elementos
                transform.SetParent(transform.root);
                // Movemos la carta al frente de todos los elementos
                transform.SetAsLastSibling();
            }
            isHovering = true;
        }
    }

    // Esta función se ejectura cuando se deja de hacer hover sobre una carta
    public void OnPointerExit(PointerEventData eventData){
        // Si no hay ninguna carta en uso y no se está arrastrando ninguna carta
        if(!gameController.isCardInUse){
            // Si no se está arrastrando la carta
            if(!isDragging){
                // Devolvemos la carta a su posición original
                transform.localScale = new Vector3(1, 1, 1);
                transform.SetParent(parentToReturnTo);
            }
            isHovering = false;
        }
    }



    // Estas funciones se ejecutan cuando se inicia un arrastre de una carta
    public void OnBeginDrag(PointerEventData eventData){
        // Si no se está hovereando una carta
        if(!isHovering && !isDragging && !gameController.isCardInUse){
            // Aumentamos el tamaño de la carta, la movemos hacia arriba para que se vea mejor, y la movemos al frente de todos los elementos
            // (normalmente se hace con el hover, pero si por alguna razón no se está hovereando, lo hacemos aquí)
            parentToReturnTo = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }
        // Llamamos al método OnCardDrag del GameController para que sepa que se está arrastrando una carta y muestre el panel de usa carta
        gameController.OnCardDrag();
        // Ajustamos el tamaño de la carta y la movemos hacia arriba para que se vea mejor
        transform.localScale = new Vector3(1, 1, 1);
        isDragging = true;
        // Desactivamos el raycastTarget de la imagen de la carta para que no se detecte el click sobre ella (y detecte otras cosas como el quad del tablero)
        image.raycastTarget = false;
        // Si la carta es de tipo Defense, instanciamos el barco, si no omitimos este paso
        if(cardDetails.CardType != null && cardDetails.CardType == "Defense" && gameController.ableToPlaceShip){
            SetShip();
        }
    }

    // Esta función se ejecuta mientras se arrastra una carta
    public void OnDrag(PointerEventData eventData){
        if(!gameController.isCardInUse || !isDragging){
            // Movemos la carta a la posición del mouse para dar el efecto de arrastre
            transform.position = Input.mousePosition;
            // Si la carta es de tipo Defense, posicionamos el barco en el tablero
            if (currentShipInstance != null)
                PositionShipOnQuad(eventData);
        }
    }

    // Esta función se ejecuta cuando se suelta una carta (se deja de arrastrar)
    public void OnEndDrag(PointerEventData eventData){
        if(!gameController.isCardInUse){
            // Llamamos al método OnCardDrop del GameController para que sepa que se soltó una carta
            gameController.OnCardDrop();
            // Devolvemos la carta a su posición original
            transform.SetParent(parentToReturnTo);
            isDragging = false;
            // Activamos nuevamente el raycastTarget de la imagen de la carta para que se detecte el click sobre ella
            image.raycastTarget = true;

            // Variable para guardar el quad sobre el que se soltó el barco, si no se soltó en un quad, por defecto es null
            Transform quadTransfrom = null;
            
            // Si la carta es de tipo Defense (hay barco), validamos si se soltó sobre un quad válido
            if (currentShipInstance != null)
                // Se guarda el quad sobre el que se soltó el barco (este lo regresa la función que valida si el barco se soltó sobre eun quad válido)
                quadTransfrom = ValidateCardDrop(eventData);


            // Si sí hay un quad válido, cambiamos el estado de los quads en donde se situó el barco
            if (quadTransfrom != null){
                // llamamos al método PlaceShipMisile del GridStateController para que cambie el estado de los quads en donde se situó el barco
                gridStateController.PlaceShipMisile(cardDetails, quadTransfrom);

                // Mandar los datos de la jugada a la base de datos

                if (!gameController.isPreparationMode)
                {
                    StartCoroutine(gameController.API.PutPlay(gameController.playNumber.ToString(), "1", (cardDetails.LengthX * cardDetails.LengthY).ToString(), gameController.gameIdClass.GameId.ToString(), cardDetails.CardId.ToString()));
                    gameController.playNumber++;
                }


                // Llamamos al método GridState del GridStateController para que actualice el estado de la 
                // cuadrícula (recuento de quads de cada tipo)
                gridStateController.GridState();
                // Debug.Log(cardDetails.LengthX);
                // Debug.Log(cardDetails.LengthY);

                Ship ship = new Ship();
                ship.Name = cardDetails.CardName;
                ship.LengthX = cardDetails.LengthX;
                ship.LengthY = cardDetails.LengthY;
                ship.quads = gridStateController.getQuadsList(ship.LengthX, ship.LengthY, quadTransfrom);
                ship.sunken = false;
                gameController.ships.Add(ship);
                gameController.audioDefenseCard.Play();

                // Si seguimos en la fase de preparación:
                if (gameController.currentState == GameController.GameState.none){
                    // Actualizamos el conteo de cartas que hay en la mano
                    gameController.cardsInHand = gameController.playerHandPreparation.transform.childCount - 1;
                    // Activar el botón para poder pasar al modo combate
                    if(gameController.cardsInHand == 0){
                        gameController.startCombatButton.interactable = true;
                    }
                }

                // Si se colocó una carta correctamente en la fase de combate
                // (aquí solo se puede poner una carta, por eso inmediatamente cambiamos al main)
                if (gameController.currentState == GameController.GameState.DefenseGrid){
                    gameController.cardsInHand = gameController.canvasCombat.transform.Find("Cards").transform.childCount - 1;
                    gameController.currentState = GameController.GameState.PCTurn;
                }
            }
            isHovering = false;
        }
    }

    // Función para instanciar el barco en el tablero
    private void SetShip(){
        // Instanciamos el barco en la posición de la carta
        currentShipInstance = Instantiate(shipPrefab, transform.position, Quaternion.identity);
        // Ajustamos la escala del barco según el tipo de carta que haya salido
        // Barcos 1x1
        if(cardDetails.LengthX == 1 && cardDetails.LengthY == 1){ 
            currentShipInstance.transform.localScale = new Vector3(.2f, .2f, .2f);
            fixPosition = new Vector3(0, 1f, 0);
        // Barcos 2x1 o 1x2
        }else if(cardDetails.LengthX == 2 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 2){
            currentShipInstance.transform.localScale = new Vector3(.3f, .3f, .3f);
            // Validamos si es horizontal o vertical
            if(cardDetails.LengthX == 2){
                // Horizontal
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-7f, 2f, 0);
            }else{
                // Vertical
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 2f, -7f);
            }
        // Barcos 3x1 o 1x3
        }else if(cardDetails.LengthX == 3 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 3){
            currentShipInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
            // Validamos si es horizontal o vertical
            if(cardDetails.LengthX == 3){
                // Horizontal
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-13f, 3f, 0);
            }else{
                // Vertical
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 3f, -13f);
            }
        // Barcos 4x1 o 1x4
        }else if(cardDetails.LengthX == 4 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 4){
            currentShipInstance.transform.localScale = new Vector3(.7f, .7f, .7f);
            // Validamos si es horizontal o vertical
            if(cardDetails.LengthX == 4){
                // Horizontal
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-20f, 5f, 0);
            }else{
                // Vertical
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 5f, -20f);
            }
        // Barcos 5x1 o 1x5
        }else if (cardDetails.LengthX == 5 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 5){
            currentShipInstance.transform.localScale = new Vector3(.7f, .7f, .9f);
            // Validamos si es horizontal o vertical
            if(cardDetails.LengthX == 5){
                // Horizontal
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-25f, 5f, 0);
            }else{
                // Vertical
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 5f, -25f);
            }
        }else if (cardDetails.LengthX == 6 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 6){
            currentShipInstance.transform.localScale = new Vector3(.7f, .7f, 1.05f);
            // Validamos si es horizontal o vertical
            if(cardDetails.LengthX == 6){
                // Horizontal
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-30f, 7.2f, 0);
            }else{
                // Vertical
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 5f, -34f);
            }
        }
    }


    // Función para validar si el barco se soltó en un quad válido
    private Transform ValidateCardDrop(PointerEventData eventData){
        // Lanzamos un raycast para detectar si el barco se soltó sobre un quad válido
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        // Si el raycast detecta un objeto y el objeto es un quad
        if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.CompareTag("GridQuad")){
            // Validamos si el barco se soltó sobre un quad válido según los criterios de la lógica del juego
            bool isValid = gridStateController.ValidateShipPlacing(cardDetails, hit.collider.transform);
            // Debug.Log($"isValid {isValid}");
            if (isValid)
            {
                // El barco se soltó sobre un quad válido, así que destruimos la carta
                Destroy(transform.parent.gameObject);
                return hit.collider.transform;
            }
            // si no es válida la posición para ubicar el barco, se destruye la instancia del barco y la carta regresa al deck
            else {
                Destroy(currentShipInstance);
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else {
            // El barco se soltó fuera de un quad válido, así que lo destruimos y la carta vuelve a su posición original
            if (currentShipInstance != null){
                Destroy(currentShipInstance);
            }
        }
        return null;
    }


    // Función para posicionar el barco en el tablero (sobre un quad)
    private void PositionShipOnQuad(PointerEventData eventData){
        // Lanzamos un raycast para detectar el quad sobre el que se soltó el barco
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        // Si el raycast detecta un objeto y el objeto es un quad
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("GridQuad"))
            {
                // instancia del quad que se está seleccionando para cambiar de estado
                Quad quad = hit.collider.GetComponent<Quad>();
                if (quad != null){
                    // // Acceder a las propiedades
                    // Debug.Log("Nombre del Quad: " + quad.name);
                    // // Modificar las propiedades
                    // quad.state = Quad.quadState.ship;

                    // Hacemos la carta un poco chica para que se vea el barco
                    transform.localScale = new Vector3(.2f, .2f, .2f);
                    // Posicionamos el barco en el centro del quad (con un pequeño ajuste dependiendo del tipo de barco)
                    currentShipInstance.transform.position = hit.collider.bounds.center + fixPosition;
                    // Ajustamos la rotación del barco si es necesario (vertical u horizontal)
                    currentShipInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // Ajusta la rotación si es necesario
                    currentShipInstance.transform.rotation *= fixRotation;
                }
            }else{
                // Si el raycast no detecta un quad, volvelmos la carta a su tamaño original
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
