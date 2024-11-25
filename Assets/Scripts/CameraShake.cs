using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Animator cameraAnim;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void Shake()
    {
        cameraAnim.SetTrigger("Shake");
        Debug.Log("Shake");
    }
}
