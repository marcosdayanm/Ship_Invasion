using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] Transform gridCenter; // Asigna el centro del tablero aquí
    [SerializeField] float rotationSpeed = 120.0f; // Velocidad a la que la cámara gira alrededor del tablero
    [SerializeField] Transform gridEnemy; // Asigna el tablero del enemigo aquí
    private bool isRotating = false; // Controla si la cámara está rotando
    private float totalRotation = 0f; // Mide el total de la rotación realizada
    private float targetRotation = 90f; // El objetivo de rotación en grados
    private bool isCameraOver = true;

    void Update()
    {
        // Inicia la rotación si se presiona el espacio y la cámara no está ya rotando
        if (Input.GetKeyDown(KeyCode.Space) && !isRotating)
        {
            Rotate();
        }

        // Realiza la rotación si isRotating es true
        if (isRotating)
        {
            DoRotateCamera();
        }
    }
    
    void Rotate()
    {
        isRotating = true;
        totalRotation = 0f;
        isCameraOver = !isCameraOver;
        if(!isCameraOver){
            gridEnemy.position = new Vector3(gridEnemy.position.x, gridEnemy.position.y + 3, gridEnemy.position.z);
        }else{
            gridEnemy.position = new Vector3(gridEnemy.position.x, gridEnemy.position.y - 3, gridEnemy.position.z);
        }
    }

    void DoRotateCamera()
    {
        // Incrementar el ángulo basado en la velocidad y el tiempo
        float angle = rotationSpeed * Time.deltaTime;
        // Asegurarse de no sobrepasar el giro deseado
        if (totalRotation + (isCameraOver ? angle : -angle) > targetRotation)
        {
            angle = targetRotation - totalRotation;
            isRotating = false; // Detiene la rotación una vez alcanzado el ángulo objetivo
        }

            // Calcular la posición orbital de la cámara
        Vector3 offset = Quaternion.Euler(isCameraOver ? -angle : angle, 0, 0) * (transform.position);
        transform.position = offset;
        // Hacer que la cámara siempre mire hacia el centro del tablero
        transform.LookAt(gridCenter.position);
        totalRotation += angle; // Actualiza el total de la rotación realizada
        if (isCameraOver && !isRotating){
            transform.rotation = Quaternion.Euler(25, 0, 0);
        }
    }
}
