using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script sirve para mover la cámara a diferentes posiciones

public class MoveCamera : MonoBehaviour
{
    // Duración de la animación de mover la cámara
    [SerializeField] float duration = 0.5f;

    // Posiciones de la cámara
    // Posición de la cámara en modo ataque
    private Vector3 attackCamera = new Vector3(0, 133, 65);
    // Posición de la cámara en modo defensa
    private Vector3 defenseCamera = new Vector3(0, 133, -80);
    // Posición de la cámara en la posición original (panorámica)
    private Vector3 originCamera = new Vector3(0, 70, -200);

    // Rotación de la cámara en la posición original
    private Quaternion originRotation = Quaternion.Euler(26, 0, 0);

    // Punto centro del grid de ataque para que la cámara esté siempre viendo ahí en modo ataque
    private Vector3 gridAttackCenter = new Vector3(0, 0, 65);

    // Punto centro del grid de defensa para que la cámara esté siempre viendo ahí en modo defensa
    private Vector3 gridDefenseCenter = new Vector3(0, 0, -80);

    // Variables para controlar si la cámara está en modo ataque o defensa
    private bool isCameraOnAttack = false;
    private bool isCameraOnDefense = false;

    // Variable para controlar si la cámara está rotando
    public bool isRotating = false;


    // Función para mover la cámara a la posición de ataque
    public void MoveCameraToAttack(){
        // Si la cámara no está en modo ataque, la movemos a la posición de ataque
        if (!isCameraOnAttack){
            StartCoroutine(MoveCameraToPosition(attackCamera, duration, gridAttackCenter));
            isCameraOnAttack = true;
            isCameraOnDefense = false; 
        }
    }

    // Función para mover la cámara a la posición de defensa
    public void MoveCameraToDefense(){
        // Si la cámara no está en modo defensa, la movemos a la posición de defensa
        if (!isCameraOnDefense){
            StartCoroutine(MoveCameraToPosition(defenseCamera, duration, gridDefenseCenter));
            isCameraOnDefense = true;
            isCameraOnAttack = false;
        }
    }

    // Función para mover la cámara a la posición original
    public void MoveCameraToOrigin(){
        var gridPosition = isCameraOnAttack ? gridAttackCenter : gridDefenseCenter;
        if (isCameraOnAttack || isCameraOnDefense){
            StartCoroutine(MoveCameraToPosition(originCamera, duration, gridPosition, true));
            isCameraOnAttack = false;
            isCameraOnDefense = false;
        }
    }


    // Corrutina para mover la cámara a una posición determinada (animación de mover la cámara)
    public IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration, Vector3 lookAtTarget, bool moveToOrigin = false) {
        // Posición y rotación inicial de la cámara
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation;
        
        // Si se quiere mover la cámara a la posición original, la rotación final es la rotación original
        if (moveToOrigin) {
            targetRotation = originRotation;
        } else {
            transform.LookAt(lookAtTarget);
            targetRotation = Quaternion.LookRotation(lookAtTarget - targetPosition);
        }
        transform.rotation = startRotation;

        // Variable para controlar el tiempo que ha pasado desde que se empezó a mover la cámara
        float time = 0;
        
        // Indica que la cámara está rotando
        isRotating = true;
        
        // Bucle que mueve la cámara de poquito en poquito en cada frame hasta llegar a la posición final (para que sea una animación fluida)
        while (time < duration) {
            // Movemos la cámara y rotamos la cámara un poquito en cada frame
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            // Aumentamos el tiempo que ha pasado desde que se empezó a mover la cámara
            time += Time.deltaTime;
            // Esperamos hasta el próximo frame
            yield return null; 
        }

        // Aseguramos que la cámara llegue a la posición y rotación final
        transform.position = targetPosition;
        transform.rotation = targetRotation;

        // Indicamos que la cámara ha terminado de rotar
        isRotating = false;
    }



}


