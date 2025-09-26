using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public static EnemyStats Instance;
    
    [Header("Base Stats (shared across all enemies)")]
    public float baseHealth = 500f;
    public float baseSpeed = 3f;
    public float baseFireRate = 5f;
    [Range(0, 0.3f)]
    public float baseBulletSpread = 0.15f;
    public float baseBulletDamage = 20f;

    private void Awake()
    {
        Instance = this;
    }

   
    private void OnEnable()
    {
        EvolutionSystem.OnEnemiesEvolve += ApplyEvolution;
    }

    private void OnDisable()
    {
        EvolutionSystem.OnEnemiesEvolve -= ApplyEvolution;
    }

    private void ApplyEvolution(EvolutionOption evo)
    {
        Debug.Log($"[OLD ENEMY BASE] {evo.optionName} → " +
                  $"Health {baseHealth}, Speed {baseSpeed}, FireRate {baseFireRate}, Spread {baseBulletSpread}, Damage {baseBulletDamage}");

        baseHealth *= (1f + evo.healthMultiplier);
        baseSpeed *= (1f + evo.speedMultiplier);
        baseFireRate *= (1f + evo.fireRateMultiplier);
        baseBulletSpread *= (1f + evo.bulletSpreadMultiplier);
        baseBulletDamage *= (1f + evo.bulletDamageMultiplier);

        Debug.Log($"[ENEMY BASE EVOLVE] {evo.optionName} → " +
                  $"Health {baseHealth}, Speed {baseSpeed}, FireRate {baseFireRate}, Spread {baseBulletSpread}, Damage {baseBulletDamage}");
    }
}
