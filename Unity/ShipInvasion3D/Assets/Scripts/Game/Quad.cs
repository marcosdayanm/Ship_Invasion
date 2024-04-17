using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script para controlar el estado de cada cuadro del grid

public class Quad : MonoBehaviour
{
    // Referencia al script que lanza proyectiles para mandar la referencia del quad que se seleccionó para lanzar el proyectil
    FireProjectile spawner;

    // Enumerador para controlar el estado de cada quad
    [SerializeField] public enum quadState { 
        none, 
        miss, 
        ship, 
        hit 
    };

    // Variable para controlar el estado actual del quad, que por defecto no tiene nada en él (none)
    [SerializeField] public quadState state = quadState.none;


    // Materiales para cambiar el color del quad dependiendo de su estado
    public Material black;
    public Material red;
    public Material blue;

    // Referencia al controlador del juego
    GameController gameController;


    // Start is called before the first frame update
    void Start()
    {
        // Se obtiene la referencia al script que lanza proyectiles
        spawner = GameObject.FindWithTag("SpawnProjectile").GetComponent<FireProjectile>();
        // Buscamos el controlador del juego para poder comparar los estados actuales de juego
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = black;
    }

    // CUando se haga click sobre un quad, se comprueba en qué estado está para poder cambiar al estado del quad apropiado
    void OnMouseDown(){
        spawner.LaunchProjectileBasedOnVelocity(transform); // lanzar proyectil
        AdjustQuadState();
    }

    // Función para cambiar el estado del quad según sea el caso 
    public void AdjustQuadState() 
    {
        if (state == quadState.ship){
            // Si hay barco en el quad, se cambia el estado a hit (que se le dio al barco)
            state = quadState.hit;
            GetComponent<Renderer>().material = red;
            }
        else if (state == quadState.hit){
            // Si ya se le dio al barco, no se hace nada
            state = quadState.hit;
            GetComponent<Renderer>().material = red;
        }
        else{
            // Si no hay barco, se cambia el estado a miss (que se falló al barco)
            state = quadState.miss;
            GetComponent<Renderer>().material = blue;
        }
    }

    // Función para indicar que se colocó un barco en el quad (cambia su estado)
    public void PlaceShip() {
        state = quadState.ship;
    }

    // Función para indicar que se está hovereando sobre el quad
    void OnMouseEnter(){
        // Aplicar hover únicamente sí está en modo ataque o defensa (es decir en espera de selección de quads)
        if(gameController.quadHoverActive){
            // Se mueve un poco hacia arriba para indicar que se está hovereando sobre el quad
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
    }

    // Función para indicar que se dejó de hoverear sobre el quad
    void OnMouseExit(){
        // Aplicar hover únicamente sí está en modo ataque o defensa (es decir en espera de selección de quads)
        if(gameController.quadHoverActive){
            // Vuelve a la posición original
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }
    }
}
