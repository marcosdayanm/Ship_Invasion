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
    [SerializeField] TMP_Text DamageByUser;
    [SerializeField] TMP_Text DamageByEnemy;
    [SerializeField] TMP_Text Winner;
    public APIConnection API = null;
    PlayerDetails user;
    Arena arena;


    // Reflejar los datos de la jugada en el Game Over
    void Start()
    {
        API = GameObject.FindWithTag("APIConnection").GetComponent<APIConnection>();
        if (API == null) {
            Debug.LogError("APIConnection component not found on the object.");
        }

        string userJson = PlayerPrefs.GetString("user");
        if (!string.IsNullOrEmpty(userJson))
            user = JsonUtility.FromJson<PlayerDetails>(userJson);
        else
            Debug.LogError("User data is empty in PlayerPrefs!");

        StartCoroutine(WhoWon());
    }

    IEnumerator WhoWon()
    {
        yield return new WaitForSeconds(0.1f);

        int sunkenShips = PlayerPrefs.GetInt("sunkenShips", 0);
        int sunkenEnemyShips = PlayerPrefs.GetInt("sunkenEnemyShips", 0);
        int activeShips = PlayerPrefs.GetInt("activeShips", 0);
        int activeEnemyShips = PlayerPrefs.GetInt("activeEnemyShips", 0);
        int damageByUser = PlayerPrefs.GetInt("damageByUser", 0);
        int damageByEnemy = PlayerPrefs.GetInt("damageByEnemy", 0);

        arena = JsonUtility.FromJson<Arena>(PlayerPrefs.GetString("arena"));

        BarcosDestruidosPlayer.text = "Barcos Destruidos: " + sunkenShips.ToString();
        BarcosRestantesPlayer.text = "Barcos Restantes: " + activeShips.ToString();
        BarcosDestruidosCPU.text = "Barcos Destruidos: " + sunkenEnemyShips.ToString();
        BarcosRestantesCPU.text = "Barcos Restantes: " + activeEnemyShips.ToString();
        DamageByUser.text = "Daño causado: " + damageByUser.ToString();
        DamageByEnemy.text = "Daño Causado: " + damageByEnemy.ToString();

        if (sunkenShips > sunkenEnemyShips)
        {
            Winner.text = "Winner: Player";
            StartCoroutine(API.EditPlayerData(user.PlayerId, user.PlayerCoins + arena.Cost + (arena.Cost * ((arena.Level / 4) + 1)), user.PlayerWins + 1, user.PlayerLosses - 1));
            yield return new WaitForSeconds(0.4f);
            StartCoroutine(API.GetPlayer(user.PlayerId));
        }
        if (sunkenShips < sunkenEnemyShips)
            Winner.text = "Winner: CPU";
        else
        {
            if (damageByUser > damageByEnemy)
            {
                Winner.text = "Winner: Player";
                StartCoroutine(API.EditPlayerData(user.PlayerId, user.PlayerCoins + arena.Cost + (arena.Cost * ((arena.Level / 4) + 1)), user.PlayerWins + 1, user.PlayerLosses - 1));
                yield return new WaitForSeconds(0.4f);
                StartCoroutine(API.GetPlayer(user.PlayerId));
            }
            else if (damageByUser < damageByEnemy)
                Winner.text = "Winner: CPU";

            else
            {
                Winner.text = "¡Es un empate!";
                StartCoroutine(API.EditPlayerData(user.PlayerId, user.PlayerCoins, user.PlayerWins, user.PlayerLosses - 1));
                yield return new WaitForSeconds(0.4f);
                StartCoroutine(API.GetPlayer(user.PlayerId));
            }

        }
    }

}
