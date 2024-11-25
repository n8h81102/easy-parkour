using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Text healthText;
    public static int healthCurrent;
    public static int healthMax ;
    private Image health;

    void Start()
    {
        health = GetComponent<Image>();
        healthCurrent = healthMax;
    }

    // Update is called once per frame
    void Update()
    {
        health.fillAmount = (float)healthCurrent / (float)healthMax;
        healthText.text = healthCurrent.ToString() + "/" + healthMax.ToString();
    }
}
