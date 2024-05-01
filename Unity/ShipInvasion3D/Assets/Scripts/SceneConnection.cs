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
        yield return StartCoroutine(API.GetArenas());
        StartCoroutine(ChangeScene("Menu"));  //Para pasar a la escena de menu
    }

    // Función para ir a la escena del menu estando en la escena de inicio (¿Login talvez?)
    public void toMenu () {
        StartCoroutine(StartGame());
    }

    // Función para ir al menú principal del juego (una vez iniciado sesión)
    public void toMainMenu () {
        StartCoroutine(ChangeScene("Menu")); //Para pasar a la escena del menu
    }

    // Función para ir a la escena de juego
    public void toGame () {
        StartCoroutine(ChangeScene("SampleScene")); //Para pasar a la escena de juego
    }

    // Función para ir a la escena de juego
    public void toEndGame () {
        StartCoroutine(ChangeScene("GameOver")); //Para pasar a la escena de juego
    }

    // Función para ir la escena de visualización de cartas
    public void toVisualizacionCartas () {
        StartCoroutine(ChangeScene("VisualizacionCartas")); //Para pasar a la escena de visualizacion de cartas
    }

    // Función para ir a la escena de creditos
    public void toCreditos () {
        StartCoroutine(ChangeScene("Creditos")); //Para pasar a la escena de creditos
    }
    
    // Función para ir a la escena de seleccion de arenas
    public void toSeleccionArena () {
        StartCoroutine(ChangeScene("SeleccionArena")); //Para pasar a la escena de selección de arena
        
    }

    IEnumerator ChangeScene(string scene){
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(scene);
    }

}