using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;



// clase custom para serializar diccionarios en JSON
[System.Serializable]
public class Serialization<TK, TV>
{
    public List<TK> keys = new List<TK>();
    public List<TV> values = new List<TV>();

    public Serialization(Dictionary<TK, TV> dict)
    {
        foreach (KeyValuePair<TK, TV> pair in dict)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}




public class APIConnection : MonoBehaviour
{
    [SerializeField] string apiURL = "http://localhost:3000";
    [SerializeField] string endpoint;
    [SerializeField] string idParameter;
    [SerializeField] TMP_Text cardName;
    private string data;
    CardDetails card;
    Cards cards;
    PlayerDetails player;

    public void Start()
    {
        // StartCoroutine(GetCards());
        // StartCoroutine(GetCard());
        // StartCoroutine(PostPlayerLogInCredentials());
        // StartCoroutine(PutPlay());
        // StartCoroutine(PutGame());
        StartCoroutine(GameEditIsPlayerWon(16));
    }

    IEnumerator GetCards()
    {
        endpoint = "/api/cards/";
        idParameter = "";
        yield return StartCoroutine(SendGetRequest());
        cards = JsonUtility.FromJson<Cards>(data);
        foreach(CardDetails card in cards.Items)
        {
            Debug.Log(card.CardName);
        }
    }

    IEnumerator GetCard()
    {
        endpoint = "/api/cards/";
        idParameter = "1";
        yield return StartCoroutine(SendGetRequest());
        card = JsonUtility.FromJson<CardDetails>(data);
        cardName.text = card.CardName;
    }


    IEnumerator PostPlayerLogInCredentials()
    {
        endpoint = "/api/players/login";
        LoginData loginData = new LoginData("Marcos", "Ship");
        string jsonData = JsonUtility.ToJson(loginData);

        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
                PlayerPrefs.SetString("user", responseData);
                // PlayerDetails playerDetails = JsonUtility.FromJson<PlayerDetails>(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }


    IEnumerator PutPlay()
    {
        endpoint = "/api/plays";
        Play play = new Play("1", "1", "1", "1", "1");
        string jsonData = JsonUtility.ToJson(play);

        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }

    IEnumerator PutGame()
    {
        endpoint = "/api/games";
        Game game = new Game("0", "3", "1");
        string jsonData = JsonUtility.ToJson(game);

        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }


    IEnumerator GameEditIsPlayerWon(int GameId)
    {
        endpoint = $"/api/games/{GameId}";
        EditIsPlayerWon game = new EditIsPlayerWon();
        string jsonData = JsonUtility.ToJson(game);

        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }


    

    IEnumerator SendGetRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiURL + endpoint + idParameter);

        yield return www.SendWebRequest();

        // If the request fails, we log the error
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Request failed: {www.error}");
        }
        else 
        {
            data = www.downloadHandler.text;
        }
    }



    IEnumerator SendPostRequest(string jsonData, Action<string> onSuccess, Action<string> onFailure)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(apiURL + endpoint, "POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) onSuccess?.Invoke(www.downloadHandler.text); 
            else onFailure?.Invoke(www.error); 

        }
    }


}


[System.Serializable]
public class LoginData
{
    public string username;
    public string password;

    public LoginData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
