using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{   
    [SerializeField] Transform enemyGrid;
    private bool isCameraOver = false;
    MoveCamera cameraController;




    // Start is called before the first frame update
    void Start()
    {
        cameraController = GameObject.FindWithTag("MainCamera").GetComponent<MoveCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !cameraController.isRotating){
            if (isCameraOver){
                cameraController.MoveCameraToOrigin();
            }else{
                cameraController.MoveCameraToOver();
            }
            isCameraOver = !isCameraOver;
            StartCoroutine(MoveGridEnemy());
        }
    }

    // public void FireToThisCoordinate(Transform quadTransform){
    //     Debug.Log("Fire to: " + quadTransform.position);
    // }

    IEnumerator MoveGridEnemy(float duration = 1.0f){
        Vector3 start = enemyGrid.position;
        Vector3 target = new Vector3(enemyGrid.position.x, isCameraOver ? enemyGrid.position.y + 1.3f : enemyGrid.position.y - 1.3f, enemyGrid.position.z);
        float time = 0;

        while (time < duration) {
            enemyGrid.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null; // Espera hasta el próximo frame
        }

        enemyGrid.position = target; // Asegura que el objeto llegue a la posición destino
    }
}
