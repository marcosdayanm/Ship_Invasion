using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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

    public void toMenu()
    {
        StartCoroutine(ChangeScene("Menu"));
    }


    IEnumerator WhoWon()
    {
        yield return new WaitForSeconds(0.4f);

        int sunkenShips = PlayerPrefs.GetInt("sunkenShips", 0);
        int sunkenEnemyShips = PlayerPrefs.GetInt("sunkenEnemyShips", 0);
        int activeShips = PlayerPrefs.GetInt("activeShips", 0);
        int activeEnemyShips = PlayerPrefs.GetInt("activeEnemyShips", 0);
        int damageByUser = PlayerPrefs.GetInt("damageByUser", 0);
        int damageByEnemy = PlayerPrefs.GetInt("damageByEnemy", 0);

        arena = JsonUtility.FromJson<Arena>(PlayerPrefs.GetString("gameArena"));

        Debug.Log(PlayerPrefs.GetString("gameArena"));
        if (arena == null)
        {
            Debug.LogError("Failed to deserialize arena data.");
            yield break;
        }

        BarcosDestruidosPlayer.text = "Barcos Destruidos: " + sunkenShips.ToString();
        BarcosRestantesPlayer.text = "Barcos Restantes: " + activeShips.ToString();
        BarcosDestruidosCPU.text = "Barcos Destruidos: " + sunkenEnemyShips.ToString();
        BarcosRestantesCPU.text = "Barcos Restantes: " + activeEnemyShips.ToString();
        DamageByUser.text = "Daño causado: " + damageByUser.ToString();
        DamageByEnemy.text = "Daño Causado: " + damageByEnemy.ToString();

        if (activeShips > activeEnemyShips)
        {
            Winner.text = $"Winner: {user.PlayerUsername}";
            StartCoroutine(API.EditPlayerData(user.PlayerId, Mathf.RoundToInt(user.PlayerCoins + arena.Cost + arena.Cost * ((arena.Level / 4.0f) + 1)), user.PlayerWins + 1, user.PlayerLosses - 1));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(API.GameEditIsPlayerWon(user.PlayerId));
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(API.GetPlayer(user.PlayerId));
            yield return new WaitForSeconds(0.2f);
        }
        else if (activeShips < activeEnemyShips)
            Winner.text = "Winner: CPU";
        else
        {
            if (damageByUser > damageByEnemy)
            {
                Winner.text = $"Winner: {user.PlayerUsername}";
                StartCoroutine(API.EditPlayerData(user.PlayerId, Mathf.RoundToInt(user.PlayerCoins + arena.Cost + arena.Cost * ((arena.Level / 4.0f) + 1)), user.PlayerWins + 1, user.PlayerLosses - 1));
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(API.GameEditIsPlayerWon(user.PlayerId));
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(API.GetPlayer(user.PlayerId));
                yield return new WaitForSeconds(0.2f);
            }
            else if (damageByUser < damageByEnemy)
                Winner.text = "Winner: CPU";

            else
            {
                Winner.text = "¡Es un empate!";
                StartCoroutine(API.EditPlayerData(user.PlayerId, user.PlayerCoins, user.PlayerWins, user.PlayerLosses - 1));
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(API.GetPlayer(user.PlayerId));
            }

        }
    }

    IEnumerator ChangeScene(string scene){
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(scene);
    }

}
