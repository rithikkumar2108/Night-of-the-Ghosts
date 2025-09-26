using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Read-Only Stats")]
    public float currentHealth = 0;
    private float startHealth;
    [Space]
    public Slider HealthSlider;
    public PlayerStats pstats;
    public GameObject GameOverPanel;

    void Start()
    {
        currentHealth = pstats.baseHealth;
        startHealth = currentHealth;
    //    HealthSlider.transform.localScale = new Vector3(HealthSlider.transform.localScale.x, HealthSlider.transform.localScale.y/100 * currentHealth, HealthSlider.transform.localScale.z);
        HealthSlider.value = 1;
    }
    void Update()
    {
        HealthSlider.value = currentHealth / startHealth;

    }
    public void TakeDamage(float d)
    {
        Debug.Log("Player Hit");
        currentHealth -= d;
        HealthSlider.value = currentHealth/startHealth;
        if(currentHealth <= 0)
        {
            EasyPeasyFirstPersonController.FirstPersonController.playerActive = false;
            GameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
