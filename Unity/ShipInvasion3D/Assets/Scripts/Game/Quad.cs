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
    public Material yellow;

    // Referencia al controlador del juego
    GameController gameController;

    // Variable que valida si el quad es seleccinable para mandar misiles
    bool validToLauchProjectiles = false;


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
        if(gameController.currentState == GameController.GameState.AttackGrid){
            if(validToLauchProjectiles){
                gameController.cardsInHand--;
                gameController.isLaunching = true;
                gameController.currentState = GameController.GameState.Main;
                gameController.OnCardDrop();

                // cambio de estado de los quads a donde se atacó
                int currentX = int.Parse(gameObject.name.Split(',')[0]);
                int currentY = int.Parse(gameObject.name.Split(',')[1]);

                // El misil es horizontal
                if(gameController.attackCardLength[0] > 1){
                    for(int i = currentX - gameController.attackCardLength[0]; i < currentX; i++){
                        if(i >= 0 && i < transform.parent.childCount){
                            Quad loopedQuad = transform.parent.GetChild(i).GetComponent<Quad>();
                            loopedQuad.AdjustQuadState();
                        }
                    }
                // El misil es vertical
                }else{
                    for(int i = currentY; i < currentY + gameController.attackCardLength[1]; i++){
                        if(i-1 >= 0 && i-1 < transform.parent.parent.childCount && currentX-1 >= 0 && currentX-1 < transform.parent.parent.GetChild(i-1).childCount){
                            Quad loopedQuad = transform.parent.parent.GetChild(i-1).GetChild(currentX-1).GetComponent<Quad>();
                            loopedQuad.AdjustQuadState();
                        }
                    }
                }        


                // StartCoroutine(gameController.LaunchProjectiles());
            }
        }
        // spawner.LaunchProjectileBasedOnVelocity(transform); // lanzar proyectil
        // AdjustQuadState();
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
            // Mueve los quads correspondientes hacia arriba dependiendo de la carta selecionada
            if(gameController.currentState == GameController.GameState.AttackGrid){
                HoverQuadsAttack();
            // Se mueve un poco hacia arriba para indicar que se está hovereando sobre el quad (en modo preparación)
            }else{
                transform.localPosition = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
            }
        }
    }

    // Función para indicar que se dejó de hoverear sobre el quad
    void OnMouseExit(){
        // Vuelve a la posición original
        if(gameController.currentState == GameController.GameState.AttackGrid){
            UnhoverQuadsAttack();
        }else{
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        }
    }



    void HoverQuadsAttack(){
        int currentX = int.Parse(gameObject.name.Split(',')[0]);
        int currentY = int.Parse(gameObject.name.Split(',')[1]);
        // El misil es horizontal
        if(gameController.attackCardLength[0] > 1){
            for(int i = currentX - gameController.attackCardLength[0]; i < currentX; i++){
                // ¿Está dentro del rango de grid?
                if(i >= 0 && i < transform.parent.childCount){
                    // Si sí, muestra efecto y sigue con el siguiente quad
                    Transform loopedQuad = transform.parent.GetChild(i);
                    loopedQuad.localPosition = new Vector3(loopedQuad.localPosition.x, 1, loopedQuad.localPosition.z);
                    validToLauchProjectiles = true;
                    gameController.quadOnAttack.Add(loopedQuad);
                    Debug.Log("Permitido mandar Misiles: " + validToLauchProjectiles);
                }else{
                    validToLauchProjectiles = false;
                    Debug.Log("No permitido mandar Misiles: " + validToLauchProjectiles);
                    break;
                }
            }
        // El misil es vertical
        }else{
            for(int i = currentY; i < currentY + gameController.attackCardLength[1]; i++){
                // ¿Está dentro del rango de grid?
                if(i-1 >= 0 && i-1 < transform.parent.parent.childCount && currentX-1 >= 0 && currentX-1 < transform.parent.parent.GetChild(i-1).childCount){
                    // Si sí, muestra efecto y sigue con el siguiente quad
                    Transform loopedQuad = transform.parent.parent.GetChild(i-1).GetChild(currentX-1);
                    loopedQuad.localPosition = new Vector3(loopedQuad.localPosition.x, 1, loopedQuad.localPosition.z);
                    validToLauchProjectiles = true;
                    gameController.quadOnAttack.Add(loopedQuad);
                    Debug.Log("Permitido mandar Misiles: " + validToLauchProjectiles);
                }else{
                    validToLauchProjectiles = false;
                    Debug.Log("No permitido mandar Misiles: " + validToLauchProjectiles);
                    break;
                }
            }
        }
    }

    void UnhoverQuadsAttack(){
        int currentX = int.Parse(gameObject.name.Split(',')[0]);
        int currentY = int.Parse(gameObject.name.Split(',')[1]);
        if(!gameController.isLaunching){
           gameController.quadOnAttack.Clear(); 
        }
        // El misil es horizontal
        if(gameController.attackCardLength[0] > 1){
            for(int i = currentX - gameController.attackCardLength[0]; i < currentX; i++){
                if(i >= 0 && i < transform.parent.childCount){
                    Transform loopedQuad = transform.parent.GetChild(i);
                    loopedQuad.localPosition = new Vector3(loopedQuad.localPosition.x, 0, loopedQuad.localPosition.z);
                }
            }
        // El misil es vertical
        }else{
            for(int i = currentY; i < currentY + gameController.attackCardLength[1]; i++){
                if(i-1 >= 0 && i-1 < transform.parent.parent.childCount && currentX-1 >= 0 && currentX-1 < transform.parent.parent.GetChild(i-1).childCount){
                    Transform loopedQuad = transform.parent.parent.GetChild(i-1).GetChild(currentX-1);
                    loopedQuad.localPosition = new Vector3(loopedQuad.localPosition.x, 0, loopedQuad.localPosition.z);
                }
            }
        }        
    }
}
