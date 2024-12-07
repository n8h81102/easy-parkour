using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoeskFire : MonoBehaviour
{
    public Transform moesk;
    public float speed;
    private float maxRange = 30f; // The max distance the bullet can travel
    private Vector3 direction; // Store the locked-in direction
    private Vector3 startPosition; // To track how far the bullet has traveled

    void Update()
    {
        // Move the bullet in the locked-in direction
        transform.position += direction * speed * Time.deltaTime;

        // Check if the bullet has traveled beyond its max range
        if (Vector3.Distance(startPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject); // Destroy bullet after traveling max range
        }
    }

    public void InitializedBullet(Transform target, float speed)
    {
        this.speed = speed;
        startPosition = transform.position; // Record the start position
        direction = (target.position - transform.position).normalized; // Lock in the direction
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
