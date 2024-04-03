using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    private Rigidbody _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
    }
    
    void OnCollisionEnter()
    {
        Instantiate(explosion,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
