using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ArenaSelectionController: MonoBehaviour
{
    PlayerDetails user;

    Arena arena;

    ArenasList arenas;

    EditPlayerDetails playerToEdit;

    public SceneConnection sceneConnection = null;
    public APIConnection API = null;



    void Start()
    {
        API = GameObject.FindWithTag("APIConnection").GetComponent<APIConnection>();
        if (API == null) {
            Debug.LogError("APIConnection component not found on the object.");
        }

        GameObject sceneConnectionObject = GameObject.FindWithTag("SceneConnection");
        if (sceneConnectionObject != null)
        {
            sceneConnection = sceneConnectionObject.GetComponent<SceneConnection>();
            if (sceneConnection == null)
                Debug.LogError("SceneConnection component not found on the object with tag 'SceneConnection'!");
        }
        else
        {
            Debug.LogError("SceneConnection object not found!");
        }

        string userJson = PlayerPrefs.GetString("user");
        if (!string.IsNullOrEmpty(userJson))
            user = JsonUtility.FromJson<PlayerDetails>(userJson);
        else
            Debug.LogError("User data is empty in PlayerPrefs!");
    }


    public void EnterArena(int arenaId)
    {
        StartCoroutine(ValidateEnterArena(arenaId));
    }


    public IEnumerator ValidateEnterArena(int arenaId)
    {
        string json = PlayerPrefs.GetString("arenas");
        if (string.IsNullOrEmpty(json))
            Debug.LogError("Arena data is empty in PlayerPrefs!");
        else
        {
            ArenasList arenas = JsonUtility.FromJson<ArenasList>(json);
            if (arenas == null || arenas.arenas == null)
                Debug.LogError("Failed to deserialize arenas or arenas list is null!");
            else if (arenaId >= arenas.arenas.Count)
                Debug.LogError("Arena index is out of range!");
            else
            {
                Arena arena = arenas.arenas[arenaId];
                if (arena == null)
                    Debug.LogError("Selected arena is null!");
                else
                {
                    Debug.Log("Arena loaded: " + arena.Name);
                    if (user == null)
                        Debug.LogError("User object is null!");
                    else if (user.PlayerCoins < arena.Cost)
                        Debug.Log("Not enough coins.");
                    else
                    {
                        Debug.Log($"Player Id: {user.PlayerId}, Player Coins: {user.PlayerCoins}, Arena Cost: {arena.Cost}, Player Wins: {user.PlayerWins}, Player Losses: {user.PlayerLosses}");
                        StartCoroutine(API.EditPlayerData(user.PlayerId, (user.PlayerCoins - arena.Cost), user.PlayerWins, user.PlayerLosses + 1));
                        yield return new WaitForSeconds(0.4f);

                        PlayerPrefs.SetString("gameArena", JsonUtility.ToJson(arena));

                        StartCoroutine(API.GetPlayer(user.PlayerId));
                        yield return new WaitForSeconds(0.4f);

                        sceneConnection.toGame();
                    }
                }
            }
        }
    }
}
