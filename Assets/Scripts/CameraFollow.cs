using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if(target != null)
        {
            if(transform.position != target.position)
            {
                Vector3 targetPosition = target.position;
                transform.position = Vector3.Lerp(transform.position, targetPosition, smooth);
            }
        }
    }

    void Update()
    {
        
    }
}
