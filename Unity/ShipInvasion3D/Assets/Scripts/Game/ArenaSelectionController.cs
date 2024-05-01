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

    APIConnection API;

    SceneConnection sceneConnection;



    void Start()
    {
        sceneConnection = GameObject.FindWithTag("SceneConnection").GetComponent<SceneConnection>();
        // API = GameObject.FindWithTag("APIConnection").GetComponent<APIConnection>();
        user = JsonUtility.FromJson<PlayerDetails>(PlayerPrefs.GetString("user"));
    }


    public void EnterArena(int arenaId)
    {
        StartCoroutine(ValidateEnterArena(arenaId));
    }



    public IEnumerator ValidateEnterArena(int arenaId)
    {
        arenas = JsonUtility.FromJson<ArenasList>(PlayerPrefs.GetString("arenas"));

        Debug.Log(arenas.items);

        arena = arenas.items[arenaId];

        if (user.PlayerCoins < arena.Cost)
        {
            // gestionar lógica de que no se puede jugar
        }
        else
        {
            StartCoroutine(sceneConnection.API.EditPlayerData(user.PlayerId, user.PlayerCoins - arena.Cost, user.PlayerWins, user.PlayerLosses));
            sceneConnection.toGame();
        }


        yield return null;

    }
}
