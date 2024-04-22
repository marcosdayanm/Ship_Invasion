using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script para controlar la conexión entre las escenas del juego

public class SceneConnection : MonoBehaviour
{   
    // Referencia a la API
    public APIConnection API = null;

    // Se obtiene la referencia de la API a ver sí la hay
    public void Start() {
        API = GetComponent<APIConnection>();
    }

    // Función para iniciar el juego (llamando a la api para obtener las cartas disponibles)
    IEnumerator StartGame() {
        yield return StartCoroutine(API.GetCards());
        SceneManager.LoadScene("Menu");  //Para pasar a la escena de menu
    }

    // Función para ir a la escena del menu estando en la escena de inicio (¿Login talvez?)
    public void toMenu () {
        StartCoroutine(StartGame());
    }

    // Función para ir al menú principal del juego (una vez iniciado sesión)
    public void toMainMenu () {
        SceneManager.LoadScene("Menu"); //Para pasar a la escena del menu
    }

    // Función para ir a la escena de juego
    public void toGame () {
        SceneManager.LoadScene("SampleScene"); //Para pasar a la escena de juego
    }

    // Función para ir a la escena de juego
    public void toEndGame () {
        SceneManager.LoadScene("EndGame"); //Para pasar a la escena de juego
    }

    // Función para ir la escena de visualización de cartas
    public void toVisualizacionCartas () {
        SceneManager.LoadScene("VisualizacionCartas"); //Para pasar a la escena de visualizacion de cartas
    }

    // Función para ir a la escena de creditos
    public void toCreditos () {
        SceneManager.LoadScene("Creditos"); //Para pasar a la escena de creditos
    }
    
    // Función para ir a la escena de seleccion de arenas
    public void toSeleccionArena () {
        SceneManager.LoadScene("SeleccionArena"); //Para pasar a la escena de selección de arena
    }
}