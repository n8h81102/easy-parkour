using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MoeskFire : MonoBehaviour
{
    public Transform moesk;
    public Transform player;
    public Transform target;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDir = (target.position - transform.position).normalized;
        transform.position += moveDir * speed * Time.deltaTime;
    }

    public void InitializedBullet(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
