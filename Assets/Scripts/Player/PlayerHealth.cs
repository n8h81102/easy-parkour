using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    private Renderer myRenderer;
    public float flashFrequency;
    public bool invincibleToTrap;
    public bool invincibleToEnemy;


    void Start()
    {
        health = 10;
        HealthBar.healthMax = health;
        HealthBar.healthCurrent = health;
        myRenderer = GetComponent<Renderer>();
        invincibleToTrap = false;
    }

    void Update()
    {
        
    }

    public void DamageToPlayer(int damage)
    {
        health -= damage;
        HealthBar.healthCurrent = health;
        if(health <= 0)
            Destroy(gameObject);
        //StartCoroutine(Flash(damage * 8, flashFrequency));
    }

    public IEnumerator InvincibleToTrapTime()
    {
        invincibleToTrap = true;
        yield return new WaitForSeconds(1.5f);
        invincibleToTrap = false;
    }

    public IEnumerator InvincibleToEnemyTime(float time)
    {
        invincibleToEnemy = true;
        yield return new WaitForSeconds(time);
        invincibleToEnemy = false;
    }

    IEnumerator Flash(int flashCnt, float frequency)
    {
        for(int i = 0; i <= flashCnt * 2; i++)
        {
            yield return new WaitForSeconds(frequency);
            myRenderer.enabled = !myRenderer.enabled;
        }
        myRenderer.enabled = true;
    }

}
