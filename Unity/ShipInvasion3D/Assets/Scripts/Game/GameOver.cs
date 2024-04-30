using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameOver : MonoBehaviour
{
    [SerializeField] TMP_Text BarcosDestruidosPlayer;
    [SerializeField] TMP_Text BarcosRestantesPlayer;
    [SerializeField] TMP_Text BarcosDestruidosCPU;
    [SerializeField] TMP_Text BarcosRestantesCPU;
    [SerializeField] TMP_Text Winner;


    // Reflejar los datos de la jugada en el Game Over
    void Start()
    {
        BarcosDestruidosPlayer.text = "Barcos Destruidos: " + PlayerPrefs.GetInt("activeShips", 0).ToString();
        BarcosRestantesPlayer.text = "Barcos Restantes: " + PlayerPrefs.GetInt("sunkenShips", 0).ToString();
        BarcosDestruidosCPU.text = "Barcos Destruidos: " + PlayerPrefs.GetInt("activeEnemyShips", 0).ToString();
        BarcosRestantesCPU.text = "Barcos Restantes: " + PlayerPrefs.GetInt("sunkenEnemyShips", 0).ToString();

        if (PlayerPrefs.GetInt("sunkenShips", 0) > PlayerPrefs.GetInt("sunkenEnemyShips", 0)){
        Winner.text = "Winner: Player";
        }
        else{
        Winner.text = "Winner: CPU";

        }
    }

}
