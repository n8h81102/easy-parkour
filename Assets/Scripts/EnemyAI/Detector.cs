using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private EnemyFSM enemyFSM;
    private Parameter parameter;

    void Start()
    {
        enemyFSM = GetComponentInParent<EnemyFSM>();
        parameter = enemyFSM.parameter;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && parameter.recoveryTime <= 0)
        {
            parameter.target = other.transform;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }
}
