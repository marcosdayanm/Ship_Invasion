using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConnection : MonoBehaviour
{   
    public APIConnection API = null;

    public void Start() {
        API = GetComponent<APIConnection>();
        // StartCoroutine(API.GetCards());
    }

    IEnumerator StartGame() {
        yield return StartCoroutine(API.GetCards());
        SceneManager.LoadScene("Menu");  //Para pasar a la escena de menu
    }

    public void toMenu () {
        StartCoroutine(StartGame());
    }

    public void toMainMenu () {
        SceneManager.LoadScene("Menu"); //Para pasar a la escena del menu
    }

    public void toGame () {
        SceneManager.LoadScene("SampleScene"); //Para pasar a la escena de juego
    }

    public void toVisualizacionCartas () {
        SceneManager.LoadScene("VisualizacionCartas"); //Para pasar a la escena de visualizacion de cartas
    }

    public void toCreditos () {
        SceneManager.LoadScene("Creditos"); //Para pasar a la escena de creditos
    }
}