using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] Rigidbody projectilePrefab;
    [SerializeField] float speed = 4;
    [SerializeField] Transform target;
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            LaunchProjectile();
        }
    }

    public void LaunchProjectile()
    {
        Rigidbody projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Vector3 toTarget = target.position - transform.position;

        float g = Physics.gravity.magnitude;
        float angle = 45; // Ángulo de lanzamiento en grados, este podría ser ajustado según la necesidad

        // Calculamos la distancia horizontal d
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float d = toTargetXZ.magnitude;

        // Calculamos la velocidad inicial requerida
        float velocity = Mathf.Sqrt(d * g / Mathf.Sin(2 * angle * Mathf.Deg2Rad));

        // Ajustamos la dirección de lanzamiento para apuntar al objetivo
        Vector3 velocityDir = toTargetXZ.normalized;
        // Calculamos los componentes de la velocidad
        float vx = velocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        float vy = velocity * Mathf.Sin(angle * Mathf.Deg2Rad);
        
        // Aplicamos la velocidad teniendo en cuenta la altura del objetivo
        float h = target.position.y - transform.position.y;
        if (h > 0)
        {
            // Ajustamos la velocidad vertical para compensar la altura del objetivo
            float extraHeight = Mathf.Sqrt(2 * h / g);
            vy += extraHeight;
        }

        // Aplicamos la velocidad al proyectil
        projectile.velocity = new Vector3(velocityDir.x * vx, vy, velocityDir.z * vx);
        projectile.transform.rotation = Quaternion.LookRotation(projectile.velocity);
    }
}
