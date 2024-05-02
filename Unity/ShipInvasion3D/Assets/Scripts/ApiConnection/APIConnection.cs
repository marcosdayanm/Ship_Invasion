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
    // [SerializeField] string apiURL = "http://localhost:3000";
    [SerializeField] string apiURL = "https://ship-invasion.onrender.com";
    // Variables para almacenar el endpoint y el parámetro de ID
    [SerializeField] string endpoint;
    [SerializeField] string idParameter;
    // Variable para almacenar la data obtenida en el request (JSON en string)
    private string data;
    // Variable que almacena la información de una carta (de tipo CardDetails)
    CardDetails card;
    // Variable que almacena la información de todas las cartas (de tipo Cards)
    Cards cards;

    ArenasList arenas;

    // Variable que almacena la información de un jugador (de tipo PlayerDetails)
    PlayerDetails player;

    public void Start()
    {
        // StartCoroutine(GetCards());
        // StartCoroutine(GetCard());
        // StartCoroutine(PostPlayerLogInCredentials());
        // StartCoroutine(PutPlay("1", "1", "3", "5", "19"));
        // StartCoroutine(PutGame("1", "3", "1"));
        // StartCoroutine(GameEditIsPlayerWon(16));
        // StartCoroutine(GetArenas());
        // StartCoroutine(EditPlayerData(5, 300, 100, 55));
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

    public IEnumerator GetArenas()
    {
        // Enpoint de la API para obtener una carta específica
        endpoint = $"/api/arenas/";
        // Parámetro de ID para obtener la carta con ID 1 (ejemplo)
        idParameter = "";
        // Se envía el request GET
        yield return StartCoroutine(SendGetRequest());
        // Se almacena la data en el PlayerPrefs para que esté disponible en toda la aplicación
        PlayerPrefs.SetString("arenas", data);
        Debug.Log($"This are the arenas: {data} ");
    }

    public IEnumerator GetPlayer(int playerId)
    {
        // Enpoint de la API para obtener una carta específica
        endpoint = $"/api/players/";
        // Parámetro de ID para obtener la carta con ID 1 (ejemplo)
        idParameter = $"{playerId}";
        // Se envía el request GET
        yield return StartCoroutine(SendGetRequest());
        // Se almacena la data en el PlayerPrefs para que esté disponible en toda la aplicación
        PlayerPrefs.SetString("user", data);
    }


    public IEnumerator PostPlayerLogInCredentials(string username, string password)
    {
    endpoint = "/api/players/login";
    LoginData loginData = new LoginData(username, password);
    string jsonData = JsonUtility.ToJson(loginData);

    yield return StartCoroutine(SendPostRequest(jsonData,
        onSuccess: (responseData) => {
            Debug.Log(responseData);
            // Verificar si la respuesta es un mensaje de error
            if (responseData.Contains("error")) {
                PlayerPrefs.DeleteKey("user");
                PlayerPrefs.SetString("error", responseData);
            } else {
                PlayerPrefs.SetString("user", responseData);
                PlayerPrefs.DeleteKey("error");
            }
        },
        onFailure: (error) => {
            Debug.LogError($"Error: {error}");
            PlayerPrefs.DeleteKey("user");
            PlayerPrefs.SetString("error", error);
        }));
    }

    public IEnumerator PostPlayerSignUpCredentials(string username, string password)
    {
    endpoint = "/api/players";
    LoginData signupData = new LoginData(username, password);
    string jsonData = JsonUtility.ToJson(signupData);

    yield return StartCoroutine(SendPostRequest(jsonData,
        onSuccess: (responseData) => {
            Debug.Log(responseData);
            // Verificar si la respuesta es un mensaje de error
            if (responseData.Contains("error")) {
                PlayerPrefs.DeleteKey("user");
                PlayerPrefs.SetString("error", responseData);
            } else {
                PlayerPrefs.SetString("user", responseData);
                PlayerPrefs.DeleteKey("error");
            }
        },
        onFailure: (error) => {
            Debug.LogError($"Error: {error}");
            PlayerPrefs.DeleteKey("user");
            PlayerPrefs.SetString("error", error);
        }));
    }
    // Función para configurar el request POST para enviar un Play
    public IEnumerator PutPlay(string PlayNumber, string IsPlayerPlay, string NumFieldsCovered, string GameId, string CardPlayedId)
    {
        // Endpoint de la API para enviar un Play
        endpoint = "/api/plays";

        // Se crea un objeto de la clase Play con la información del Play
        Play play = new Play(PlayNumber, IsPlayerPlay, NumFieldsCovered, GameId, CardPlayedId);

        // Se convierte el objeto en un JSON en string para que pueda ser enviado en el request
        string jsonData = JsonUtility.ToJson(play);

        // Se envía el request POST y se espera a que termine, dependiedno de la respuesta, se ejecuta una función anónima en caso de exito o fracaso
        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }

    // Función para configurar el request POST para enviar un Game
    public IEnumerator PutGame(string IsPlayerWon, string PlayerId, string ArenaId)
    {
        // Endpoint de la API para enviar un Game
        endpoint = "/api/games";
        // Se crea un objeto de la clase Game con la información del Game
        Game game = new Game(IsPlayerWon, PlayerId, ArenaId);
        // Se convierte el objeto en un JSON en string para que pueda ser enviado en el request
        string jsonData = JsonUtility.ToJson(game);

// Se envía el request POST y se espera a que termine, dependiedno de la respuesta, se ejecuta una función anónima en caso de exito o fracaso
        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {
                PlayerPrefs.SetString("gameId", responseData);
                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }

    // Función para configurar el request POST para modificar el atributo isPlayerWon de un Game en caso de que el jugador lo haya ganado
    public IEnumerator GameEditIsPlayerWon(int GameId)
    {
        // Endpoint de la API para modificar el atributo isPlayerWon de un Game
        endpoint = $"/api/games/{GameId}";

        // Se crea un objeto de la clase EditIsPlayerWon con el atributo isPlayerWon en true
        EditIsPlayerWon game = new EditIsPlayerWon();
        string jsonData = JsonUtility.ToJson(game);

// Se envía el request POST y se espera a que termine, dependiedno de la respuesta, se ejecuta una función anónima en caso de exito o fracaso
        yield return StartCoroutine(SendPostRequest(jsonData,
            onSuccess: (responseData) => {

                Debug.Log(responseData);
            },
            onFailure: (error) => {
                Debug.LogError($"Error: {error}");
            }));
    }


        public IEnumerator EditPlayerData(int PlayerId, int PlayerCoins, int PlayerWins, int PlayerLosses)
        {
            // Endpoint de la API para modificar el atributo isPlayerWon de un Game
            endpoint = $"/api/player/edit-data";

            EditPlayerDetails player = new EditPlayerDetails(PlayerId, PlayerCoins, PlayerWins, PlayerLosses);
            string jsonData = JsonUtility.ToJson(player);
            Debug.Log("JSON being sent: " + jsonData);

    // Se envía el request POST y se espera a que termine, dependiendo de la respuesta, se ejecuta una función anónima en caso de exito o fracaso
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


    // Función para enviar un request POST, ésta función toma como argumentos el JSON en string, una función anónima que se ejecutará si el request es exitoso y otra función anónima que se ejecutará si el request falla
    IEnumerator SendPostRequest(string jsonData, Action<string> onSuccess, Action<string> onFailure)
    {
        // Se crea un objeto de la clase UnityWebRequest para enviar el request POST con la URL de la API y el endpoint
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(apiURL + endpoint, "POST"))
        {
            // Se convierte el JSON en string en un array de bytes para que pueda ser enviado en el request (método de envío a través de UnityWebRequest)
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            // Se configura el request con el array de bytes y el header Content-Type
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Se envía el request y se espera a que termine
            yield return www.SendWebRequest();

            // Si el request fue exitoso, se ejecuta la función onSuccess, si no se ejecuta la función onFailure
            if (www.result == UnityWebRequest.Result.Success) onSuccess?.Invoke(www.downloadHandler.text); 
            else {
                PlayerPrefs.SetString("errorMsg", www.downloadHandler.text);
                Debug.Log("Failed: " + www.error);
                Debug.Log("Response: " + www.downloadHandler.text); 
                onFailure?.Invoke(www.error); 
            }

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
