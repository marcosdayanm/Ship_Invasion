using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    FireProjectile spawner;
    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindWithTag("SpawnProjectile").GetComponent<FireProjectile>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown(){
        spawner.LaunchProjectileBasedOnVelocity(transform);
    }

    void OnMouseEnter(){
        GetComponent<Renderer>().material.color = Color.red;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }

    void OnMouseExit(){
        GetComponent<Renderer>().material.color = Color.white;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
    }
}
