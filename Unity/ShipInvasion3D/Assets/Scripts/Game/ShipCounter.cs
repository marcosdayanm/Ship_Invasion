using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ShipCounter : 
                            MonoBehaviour,
                            IPointerEnterHandler,  // Necesario para detectar el puntero entrando en el objeto
                            IPointerExitHandler // Necesario para detectar el puntero saliendo del objeto
{
    public int[] vertical = {0,0,0,0,0};
    public int[] horizontal = {0,0,0,0,0};
    public int one = 0;

    GameController gameController;
    
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        CountShips();
    }

    public void CountShips(){
        vertical = new int[5];
        horizontal = new int[5];
        one = 0;
        for(int i = 0; i < gameController.enemyShips.Count; i++){
            // Barco horizontal
            if(gameController.enemyShips[i].LengthX > 1 && gameController.enemyShips[i].LengthY == 1 && !gameController.enemyShips[i].sunken){
                horizontal[gameController.enemyShips[i].LengthX - 2]++;
            // Barco Vertical
            }else if(gameController.enemyShips[i].LengthY > 1 && gameController.enemyShips[i].LengthX == 1 && !gameController.enemyShips[i].sunken){
                vertical[gameController.enemyShips[i].LengthY - 2]++;
            }else if(gameController.enemyShips[i].sunken){
                one++;
            }
        }
        SetShipsCouter();
    }

    void SetShipsCouter(){
        for (int i = 2; i < 7; i++){
            if(vertical[i - 2] > 0){
                transform.Find($"{i}V").Find("Amount").GetComponent<TMP_Text>().text = "<b>" + vertical[i - 2].ToString() + "x</b>";
            }else{
                transform.Find($"{i}V").Find("Amount").GetComponent<TMP_Text>().text = vertical[i - 2].ToString() + "x";
            }
            if(horizontal[i - 2] > 0){
                transform.Find($"{i}H").Find("Amount").GetComponent<TMP_Text>().text = "<b>" + horizontal[i - 2].ToString() + "x</b>";
            }else{
                transform.Find($"{i}H").Find("Amount").GetComponent<TMP_Text>().text = horizontal[i - 2].ToString() + "x";
            }
        }
        if(one > 0){
            transform.Find("1H").Find("Amount").GetComponent<TMP_Text>().text = $"<b>{one}x</b>";
        }else{
            transform.Find("1H").Find("Amount").GetComponent<TMP_Text>().text = $"{one}x";
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        transform.localPosition = new Vector3(728, transform.localPosition.y, transform.localPosition.z);
    }

    public void OnPointerExit(PointerEventData eventData){
        transform.localPosition = new Vector3(1135, transform.localPosition.y, transform.localPosition.z);
    }
}
