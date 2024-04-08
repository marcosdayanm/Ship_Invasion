using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] float duration = 1.0f;
    private Vector3 attackCamera = new Vector3(0, 133, 65);
    private Vector3 defenseCamera = new Vector3(0, 133, -80);
    private Vector3 originCamera = new Vector3(0, 70, -200);
    private Quaternion originRotation = Quaternion.Euler(26, 0, 0);
    private Vector3 gridAttackCenter = new Vector3(0, 0, 65);
    private Vector3 gridDefenseCenter = new Vector3(0, 0, -80);
    private bool isCameraOnAttack = false;
    private bool isCameraOnDefense = false;
    public bool isRotating = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCameraToAttack(){
        if (!isCameraOnAttack){
            StartCoroutine(MoveCameraToPosition(attackCamera, duration, gridAttackCenter));
            isCameraOnAttack = true;
            isCameraOnDefense = false; 
        }
    }

    public void MoveCameraToDefense(){
        if (!isCameraOnDefense){
            StartCoroutine(MoveCameraToPosition(defenseCamera, duration, gridDefenseCenter));
            isCameraOnDefense = true;
            isCameraOnAttack = false;
        }
    }

    public void MoveCameraToOrigin(){
    var gridPosition = isCameraOnAttack ? gridAttackCenter : gridDefenseCenter;
    if (isCameraOnAttack || isCameraOnDefense){
        StartCoroutine(MoveCameraToPosition(originCamera, duration, gridPosition, true));
        isCameraOnAttack = false;
        isCameraOnDefense = false;
    }
}


    public IEnumerator MoveCameraToPosition(Vector3 targetPosition, float duration, Vector3 lookAtTarget, bool moveToOrigin = false) {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation;
        
        if (moveToOrigin) {
            targetRotation = originRotation;
        } else {
            transform.LookAt(lookAtTarget);
            targetRotation = Quaternion.LookRotation(lookAtTarget - targetPosition);
        }

        transform.rotation = startRotation;

        float time = 0;
        isRotating = true;

        while (time < duration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null; 
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
        isRotating = false;
    }



}


