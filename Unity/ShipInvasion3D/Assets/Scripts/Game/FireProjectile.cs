using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    // Prefab del proyectil a lanzar
    [SerializeField] Rigidbody projectilePrefab;
    // Velocidad de lanzamiento del proyectil
    [SerializeField] float speed = 120;


    // Esta función sirve para lanzar el proyectil hacia el objetivo basado en un ángulo de lanzamiento (ángulo fijo)
    public void LaunchProjectileBasedOnAngle(Transform target){
        // Instanciamos el proyectil
        Rigidbody projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        // Calculamos el vector que apunta al objetivo
        Vector3 toTarget = target.position - transform.position;

        // Gravedad del mundo
        float g = Physics.gravity.magnitude;
        float angle = 45; // Ángulo de lanzamiento en grados

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
        if (h > 0){
            // Ajustamos la velocidad vertical para compensar la altura del objetivo
            float extraHeight = Mathf.Sqrt(2 * h / g);
            vy += extraHeight;
        }

        // Aplicamos la velocidad al proyectil
        projectile.velocity = new Vector3(velocityDir.x * vx, vy, velocityDir.z * vx);
        projectile.transform.rotation = Quaternion.LookRotation(projectile.velocity);
    }


    // Esta función sirve para lanzar el proyectil hacia el objetivo basado en la velocidad de lanzamiento (velocidad fija)
    public void LaunchProjectileBasedOnVelocity(Transform target)
    {
        // Instanciamos el proyectil
        Rigidbody projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        // Calculamos el vector que apunta al objetivo
        Vector3 toTarget = target.position - transform.position;

        // Gravedad del mundo
        float g = Physics.gravity.magnitude;
        float desiredSpeed = speed; // Velocidad de lanzamiento deseada

        // Calculamos la distancia horizontal d
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);
        float d = toTargetXZ.magnitude;

        // Calculamos el ángulo necesario para la velocidad deseada
        float angle = Mathf.Asin(g * d / Mathf.Pow(desiredSpeed, 2)) / 2.0f;

        if (float.IsNaN(angle)) // Si el ángulo no es real, significa que la velocidad deseada es demasiado baja para alcanzar el objetivo
        {
            Debug.LogError("La velocidad deseada es demasiado baja para alcanzar el objetivo. Ajustando a la velocidad mínima necesaria...");
            // Ajusta la velocidad al mínimo necesario para alcanzar el objetivo a 45 grados, o maneja este caso como prefieras
            desiredSpeed = Mathf.Sqrt(d * g / Mathf.Sin(2 * 45 * Mathf.Deg2Rad));
            angle = 45 * Mathf.Deg2Rad; // Volvemos a 45 grados en radianes como nuestro ángulo de compromiso
        }

        // Ajustamos la dirección de lanzamiento para apuntar al objetivo
        Vector3 velocityDir = toTargetXZ.normalized;
        // Calculamos los componentes de la velocidad
        float vx = desiredSpeed * Mathf.Cos(angle);
        float vy = desiredSpeed * Mathf.Sin(angle);
        
        // Aplicamos la velocidad teniendo en cuenta la altura del objetivo
        float h = target.position.y - transform.position.y;
        if (h > 0){
            // Ajustamos la velocidad vertical para compensar la altura del objetivo
            float extraHeight = Mathf.Sqrt(2 * h / g);
            vy += extraHeight;
        }

        // Aplicamos la velocidad al proyectil
        projectile.velocity = new Vector3(velocityDir.x * vx, vy, velocityDir.z * vx);
        projectile.transform.rotation = Quaternion.LookRotation(projectile.velocity);
    }
}
