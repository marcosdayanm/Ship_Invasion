using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] float duration = 1.5f;
    private Vector3 overCamera = new Vector3(0, 133, 65);
    private Vector3 originCamera = new Vector3(0, 70, -200);
    private Vector3 gridCenter = new Vector3(0, 0, 65);
    private bool isCameraOver = false;
    public bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCameraToOver(){
        if (!isCameraOver){
            StartCoroutine(MoveCameraToPosition(overCamera, duration));
            isCameraOver = true;
        }
    }

    public void MoveCameraToOrigin(){
        if (isCameraOver){
            StartCoroutine(MoveCameraToPosition(originCamera, duration));
            isCameraOver = false;
        }
    }

    public IEnumerator MoveCameraToPosition(Vector3 target, float duration) {
        Vector3 start = transform.position;
        float time = 0;
        isRotating = true;

        while (time < duration) {
            transform.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            transform.LookAt(gridCenter);
            yield return null; // Espera hasta el próximo frame
        }

        transform.position = target; // Asegura que el objeto llegue a la posición destino
        isRotating = false;
        if (isCameraOver){
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
