using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfoDisplay : MonoBehaviour
{
    PlayerDetails user;
    public TextMeshProUGUI monedasText;
    public TextMeshProUGUI usuarioText;

    void Start()
    {
        user = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));
        
        if (monedasText != null)
        {
            monedasText.text = "Monedas: " + user.PlayerCoins;
        }

        if (usuarioText != null)
        {
            usuarioText.text = "Usuario: " + user.PlayerUsername;
        }
    }


}
