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
    [HideInInspector] public Transform lastCellCard = null;
    GameController gameController;
    public Image image;

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
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentToReturnTo);
        isDragging = false;
        gameController.OnCardDrop();
        image.raycastTarget = true;
    }
}
