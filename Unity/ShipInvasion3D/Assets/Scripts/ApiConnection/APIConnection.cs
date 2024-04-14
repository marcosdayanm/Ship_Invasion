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
    // Variable para almacenar la URL de la API
    [SerializeField] string apiURL = "http://localhost:3000";
    // Variables para almacenar el endpoint y el parámetro de ID
    [SerializeField] string endpoint;
    [SerializeField] string idParameter;
    // Variable para almacenar la data obtenida en el request (JSON en string)
    private string data;
    // Variable que almacena la información de una carta (de tipo CardDetails)
    CardDetails card;
    // Variable que almacena la información de todas las cartas (de tipo Cards)
    Cards cards;
    // Variable que almacena la información de un jugador (de tipo PlayerDetails)
    PlayerDetails player;

    public void Start()
    {
        // StartCoroutine(GetCards());
        // StartCoroutine(GetCard());
        // StartCoroutine(PostPlayerLogInCredentials());
        // StartCoroutine(PutPlay());
        // StartCoroutine(PutGame());
        // StartCoroutine(GameEditIsPlayerWon(16));
    }

    // Función para configurar el request GET para obtener TODAS las Cartas
    public IEnumerator GetCards()
    {
        // Enpoint de la API para obtener todas las cartas
        endpoint = "/api/cards/";
        // Parámetro de ID vacío porque no se necesita en este endpoint
        idParameter = "";
        // Se envía el request GET
        yield return StartCoroutine(SendGetRequest());
        // Se deserializa el JSON obtenido en un objeto Cards (se convierte de JSON a un objeto de la clase Cards)
        cards = JsonUtility.FromJson<Cards>(data);
        // Se almacena la data en el PlayerPrefs para que esté disponible en toda la aplicación
        PlayerPrefs.SetString("cards", data);
    }

    // Función para configurar el request GET para obtener una Carta específica
    IEnumerator GetCard()
    {
        // Enpoint de la API para obtener una carta específica
        endpoint = "/api/cards/";
        // Parámetro de ID para obtener la carta con ID 1 (ejemplo)
        idParameter = "1";
        // Se envía el request GET
        yield return StartCoroutine(SendGetRequest());
        // Se deserializa el JSON obtenido en un objeto CardDetails (se convierte de JSON a un objeto de la clase CardDetails)
        card = JsonUtility.FromJson<CardDetails>(data);
        // Se almacena la data en el PlayerPrefs para que esté disponible en toda la aplicación
        PlayerPrefs.SetString(card.CardName, data);
    }


    // Función para configurar el request POST para enviar las credenciales de inicio de sesión de un jugador
    IEnumerator PostPlayerLogInCredentials()
    {
        // Enpoint de la API para enviar las credenciales de inicio de sesión
        endpoint = "/api/players/login";
        // Se crea un objeto de la clase LoginData con las credenciales de inicio de sesión
        LoginData loginData = new LoginData("Marcos", "Ship");
        // Se convierte el objeto en un JSON en string para que pueda ser enviado en el request
        string jsonData = JsonUtility.ToJson(loginData);

        // Se envía el request POST
        yield return StartCoroutine(SendPostRequest(jsonData,
            // Se pasa una función anónima que se ejecutará si el request es exitoso
            onSuccess: (responseData) => {
                Debug.Log(responseData);
                // Se almacena la data en el PlayerPrefs para que esté disponible en toda la aplicación
                PlayerPrefs.SetString("user", responseData);

                // PlayerDetails playerDetails = JsonUtility.FromJson<PlayerDetails>(responseData);
            },
            // Se pasa una función anónima que se ejecutará si el request falla
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


    

    // Función para enviar un request GET
    IEnumerator SendGetRequest()
    {
        // Se crea un objeto de la clase UnityWebRequest para enviar el request GET con la URL de la API y el endpoint
        UnityWebRequest www = UnityWebRequest.Get(apiURL + endpoint + idParameter);

        // Se envía el request y se espera a que termine
        yield return www.SendWebRequest();

        // Si el request fue exitoso, se almacena la data obtenida en la variable data, si no se muestra un mensaje de error
        if(www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Request failed: {www.error}");
        }
        else 
        {
            data = www.downloadHandler.text;
        }
    }


    // Función para enviar un request POST
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


// Clase para construir el objeto con las credenciales de un jugador para iniciar sesión
[System.Serializable]
public class LoginData
{
    public string username;
    public string password;

    // Constructor de la clase
    public LoginData(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
