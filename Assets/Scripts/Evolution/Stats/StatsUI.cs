using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerStatsText;
    [SerializeField] private TMP_Text enemyStatsText;

    private void OnEnable()
    {
       UpdateStats();
    }

    private void Start()
    {
        UpdateStats();
    }

   

    public void UpdateStats()
    {
        if (PlayerStats.Instance != null)
        {
            playerStatsText.text =
                $"PLAYER STATS\n" +
                $"Health: {PlayerStats.Instance.baseHealth:F1}\n" +
                $"Speed: {PlayerStats.Instance.baseSpeed:F2}\n" +
                $"Fire Rate: {PlayerStats.Instance.baseFireRate:F2}\n" +
                $"Spread: {PlayerStats.Instance.baseBulletSpread:F2}\n" +
                $"Damage: {PlayerStats.Instance.baseBulletDamage:F1}";
        }

        if (EnemyStats.Instance != null)
        {
            enemyStatsText.text =
                $"ENEMY STATS\n" +
                $"Health: {EnemyStats.Instance.baseHealth:F1}\n" +
                $"Speed: {EnemyStats.Instance.baseSpeed:F2}\n" +
                $"Fire Rate: {EnemyStats.Instance.baseFireRate:F2}\n" +
                $"Spread: {EnemyStats.Instance.baseBulletSpread:F2}\n" +
                $"Damage: {EnemyStats.Instance.baseBulletDamage:F1}";
        }
    }
}
