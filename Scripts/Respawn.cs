using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private PlayerMove PlayerMove;
    // Start is called before the first frame update
    void Start()
    {
        PlayerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
            PlayerMove.respawnPoint.position = transform.position;
        //Debug.Log("更新重生點" + transform.position);
    }
}
