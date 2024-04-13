using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CardController :  
                                MonoBehaviour, 
                                IPointerEnterHandler, 
                                IPointerExitHandler, 
                                IBeginDragHandler, 
                                IDragHandler, 
                                IEndDragHandler
{

    private bool isDragging = false;
    private bool isHovering = false;
    [HideInInspector] public Transform parentToReturnTo = null;
    [SerializeField] public GameObject shipPrefab;
    [HideInInspector] public Transform lastCellCard = null;
    GameController gameController;
    [SerializeField] public Image image;
    [HideInInspector] public CardDetails cardDetails;
    private GameObject currentShipInstance;
    private Vector3 fixPosition;
    private Quaternion fixRotation;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(!gameController.isCardInUse){
            if(!isDragging && !gameController.isCardDragging){
                transform.position = new Vector3(transform.position.x, transform.position.y + 90, transform.position.z);
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                parentToReturnTo = transform.parent;
                transform.SetParent(transform.root);
                transform.SetAsLastSibling();
            }
            isHovering = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(!gameController.isCardInUse){
            if(!isDragging){
                transform.localScale = new Vector3(1, 1, 1);
                transform.SetParent(parentToReturnTo);
            }
            isHovering = false;
        }
    }



    public void OnBeginDrag(PointerEventData eventData){
        if(!isHovering){
            parentToReturnTo = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }
        transform.localScale = new Vector3(1, 1, 1);
        isDragging = true;
        gameController.OnCardDrag();
        image.raycastTarget = false;
        if(cardDetails.CardType != null && cardDetails.CardType == "Defense"){
            SetShip();
        }
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
        if (currentShipInstance != null)
            PositionShipOnQuad(eventData);
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentToReturnTo);
        isDragging = false;
        gameController.OnCardDrop();
        image.raycastTarget = true;
        
        if (currentShipInstance != null)
            ValidateCardDrop(eventData);
    }

    private void SetShip(){
        currentShipInstance = Instantiate(shipPrefab, transform.position, Quaternion.identity);
        if(cardDetails.LengthX == 1 && cardDetails.LengthY == 1){
            currentShipInstance.transform.localScale = new Vector3(.2f, .2f, .2f);
            fixPosition = new Vector3(0, 1f, 0);
        }else if(cardDetails.LengthX == 2 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 2){
            currentShipInstance.transform.localScale = new Vector3(.3f, .3f, .3f);
            if(cardDetails.LengthX == 2){
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-7f, 2f, 0);
            }else{
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 2f, -7f);
            }
        }else if(cardDetails.LengthX == 3 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 3){
            currentShipInstance.transform.localScale = new Vector3(.5f, .5f, .5f);
            if(cardDetails.LengthX == 3){
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-13f, 3f, 0);
            }else{
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 3f, -13f);
            }
        }else if(cardDetails.LengthX == 4 && cardDetails.LengthY == 1 || cardDetails.LengthX == 1 && cardDetails.LengthY == 4){
            currentShipInstance.transform.localScale = new Vector3(.7f, .7f, .7f);
            if(cardDetails.LengthX == 4){
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-20f, 5f, 0);
            }else{
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 5f, -20f);
            }
        }else{
            currentShipInstance.transform.localScale = new Vector3(.7f, .7f, .9f);
            
            if(cardDetails.LengthX == 5){
                fixRotation = Quaternion.Euler(0, 90, 0);
                fixPosition = new Vector3(-25f, 5f, 0);
            }else{
                fixRotation = Quaternion.Euler(0, 0, 0);
                fixPosition = new Vector3(0, 5f, -25f);
            }
        }
    }

    private void ValidateCardDrop(PointerEventData eventData){
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.CompareTag("GridQuad"))
        {
            // El barco se soltó sobre un quad válido, así que destruimos la carta
            Destroy(transform.parent.gameObject); // Destruye la carta
        }
        else
        {
            // El barco se soltó fuera de un quad válido, así que lo destruimos
            if (currentShipInstance != null)
            {
                Destroy(currentShipInstance);
            }
        }
    }

    private void PositionShipOnQuad(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("GridQuad"))
            {

                Quad quad = hit.collider.GetComponent<Quad>();
                if (quad != null)
                {
                    // Acceder a las propiedades
                    Debug.Log("Nombre del Quad: " + quad.name);

                    // Modificar las propiedades
                    quad.state = Quad.quadState.ship;


                transform.localScale = new Vector3(.2f, .2f, .2f);
                currentShipInstance.transform.position = hit.collider.bounds.center + fixPosition;
                currentShipInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // Ajusta la rotación si es necesario
                currentShipInstance.transform.rotation *= fixRotation;
                
                }
            }else{
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
