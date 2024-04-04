using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


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
    [SerializeField] string apiURL = "localhost:3000";
    [SerializeField] string endpoint;
    [SerializeField] string idParameter;
    [SerializeField] TMP_Text cardName;
    private string data;
    CardDetails card;
    Cards cards;
    PlayerDetails player;

    public void Start()
    {
        StartCoroutine(GetCards());
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
        endpoint = "/api/player/";
        idParameter = "";

        Dictionary<string, string> data = new Dictionary<string, string>
        {
            {"username", "player1"},
            {"password", "password1"}
        };

        yield return StartCoroutine(SendPostRequest(data));
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


//     IEnumerator SendPostRequest(new Dictionary<string, string> data)
//     {
//         // Convertir el diccionario a JSON con nuestra clase personalizada
//         string jsonData = JsonUtility.ToJson(new Serialization<string, string>(data));


//         UnityWebRequest www = new UnityWebRequest(apiURL + endpoint, "POST");
//         byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
//         www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
//         www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
//         www.SetRequestHeader("Content-Type", "application/json");

//         // Enviar la solicitud
//         yield return www.SendWebRequest();

//         if(www.result != UnityWebRequest.Result.Success) Debug.Log($"Request failed: {www.error}");
//         else
//         {
//             string responseData = www.downloadHandler.text;
//             Debug.Log($"Response: {responseData}");
//             return responseData;
//         }
//     }

    // Método de Gil
    IEnumerator SendPostRequest(new Dictionary<string, string> data)
    {
        string jsonData = JsonUtility.ToJson(new Serialization<string, string>(data));
        UnityWebRequest www = new UnityWebRequest.Put(apiURL + endpoint, jsonData)
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success) Debug.Log($"Request failed: {www.error}");
            else
            {
                string responseData = www.downloadHandler.text;
                Debug.Log($"Response: {responseData}");
                return responseData;
            }
        }
    }

}

