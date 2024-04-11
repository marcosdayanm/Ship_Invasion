using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    FireProjectile spawner;
    [SerializeField]
    public enum quadState { none, miss, ship, hit };


    [SerializeField]
    public quadState state = quadState.none;
    public Material black;
    public Material red;
    public Material blue;


    // Start is called before the first frame update
    void Start()
    {
        spawner = GameObject.FindWithTag("SpawnProjectile").GetComponent<FireProjectile>();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = black;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown(){
        spawner.LaunchProjectileBasedOnVelocity(transform);
        if (state == quadState.ship)
        {
            state = quadState.hit;
            GetComponent<Renderer>().material = red;
        }
            
        else 
        {
            state = quadState.miss;
            GetComponent<Renderer>().material = blue;
        }
        
        
    }

    void OnMouseEnter(){
        // GetComponent<Renderer>().material.color = Color.blue;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }

    void OnMouseExit(){
        // GetComponent<Renderer>().material.color = Color.white;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
    }
}
