using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class APIConnection : MonoBehaviour
{
    [SerializeField] string apiURL = "localhost:3000";
    [SerializeField] string endpoint;
    [SerializeField] string idParameter;
    [SerializeField] TMP_Text cardName;
    private string data;
    CardDetails card;
    Cards cards;


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
}