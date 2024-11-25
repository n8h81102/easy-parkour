using System.Runtime.CompilerServices;
using System.Diagnostics.Contracts;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class OneWayPlatform : MonoBehaviour
{
    private Transform player;
    private BoxCollider2D myCollider2D;
    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        target = player.transform.GetChild(1);
        myCollider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHeight();
    }
    void CheckHeight()
    {
        if(target.position.y <= transform.position.y || Input.GetKey(KeyCode.S))
            myCollider2D.enabled = false;
        else 
            myCollider2D.enabled = true;
    } 


}
