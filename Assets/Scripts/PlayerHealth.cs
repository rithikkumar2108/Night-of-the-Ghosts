using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Read-Only Stats")]
    public float currentHealth = 100;
    private float startHealth;
    [Space]
    public Slider HealthSlider;

    void Start()
    {
        startHealth = currentHealth;
        HealthSlider.transform.localScale = new Vector3(HealthSlider.transform.localScale.x, HealthSlider.transform.localScale.y/100 * currentHealth, HealthSlider.transform.localScale.z);
        HealthSlider.value = 1;
    }
    public void TakeDamage(float d)
    {
        Debug.Log("Player Hit");
        currentHealth -= d;
        HealthSlider.value = currentHealth/startHealth;
    }
}
