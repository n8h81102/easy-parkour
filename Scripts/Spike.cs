using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spike : MonoBehaviour
{
    public PlayerHealth PlayerHealth;
    public PlayerMove PlayerMove;
    public Transform player;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        PlayerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        PlayerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && !PlayerHealth.invincibleToTrap)
        {   
            StartCoroutine(PlayerHealth.InvincibleToTrapTime());
            StartCoroutine(TimeStop());
            PlayerMove.KnockBack(0);
            PlayerHealth.DamageToPlayer(damage);
        }
    }

    IEnumerator TimeStop()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(damage * 0.1f);
        Time.timeScale = 1;
    }

}
