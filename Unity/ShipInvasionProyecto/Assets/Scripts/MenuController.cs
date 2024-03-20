//Version 1 
// 19 mar 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ShipInvasion");
    }

}
