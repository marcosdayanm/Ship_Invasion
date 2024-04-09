using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConnection : MonoBehaviour
{   
    public APIConnection API;
public void toMenu () {
SceneManager.LoadScene("Menu"); //Para pasar a la escena de menu
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