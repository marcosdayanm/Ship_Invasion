using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfoDisplay : MonoBehaviour
{
    PlayerDetails user;
    public TextMeshProUGUI usuarioText;
    public TextMeshProUGUI monedasText;
    public TextMeshProUGUI ganadasText;
    public TextMeshProUGUI perdidasText;

    void Start()
    {
        user = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));

        usuarioText.text = "Usuario: " + user.PlayerUsername;
        monedasText.text = "Monedas: " + user.PlayerCoins;
        ganadasText.text = "Ganadas: " + user.PlayerWins;
        perdidasText.text = "Perdidas: " + user.PlayerLosses;

    }


}
