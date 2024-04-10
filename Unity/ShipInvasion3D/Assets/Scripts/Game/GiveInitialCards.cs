using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveInitialCards : MonoBehaviour
{
    GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveCards(){
         
    }
}
