using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoeskFire : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth playerHealth;
    public Rigidbody2D myRigidbody;
    public Transform moesk;
    public float speed;
    private float maxRange = 30f; // The max distance the bullet can travel
    private Vector3 direction; // Store the locked-in direction
    private Vector3 startPosition; // To track how far the bullet has traveled
    [SerializeField] private int damage;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }
    void Update()
    {
        // Move the bullet in the locked-in direction
        Vector2 bulletSpeed = new Vector2(direction.x * speed, direction.y * speed);
        this.myRigidbody.linearVelocity = bulletSpeed;

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

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(!playerHealth.invincibleToEnemy)
            {
                playerHealth.StartCoroutine(playerHealth.InvincibleToEnemyTime(1));
                playerHealth.DamageToPlayer(damage);
            }
            
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}

public class MoeskFire2 : MonoBehaviour
{
    
}
