using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public EasyPeasyFirstPersonController.FirstPersonController Player;
    [Header("Stats")]
    public float baseHealth = 500f;
    public float baseSpeed = 3f;
    public float baseFireRate = 5f;
    [Range(0, 0.3f)]
    public float baseBulletSpread = 0.15f;
    public float baseBulletDamage = 20f;

    public int magazineCount = 20;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        EvolutionSystem.OnPlayerEvolve += ApplyEvolution;
    }

    private void OnDisable()
    {
        EvolutionSystem.OnPlayerEvolve -= ApplyEvolution;
    }

    private void ApplyEvolution(EvolutionOption evo)
    {
        Debug.Log($"[OLD PLAYER] {evo.optionName} → " +
                  $"Health {baseHealth}, Speed {baseSpeed}, FireRate {baseFireRate}, Spread {baseBulletSpread}, Damage {baseBulletDamage}");

        baseHealth *= (1f + evo.healthMultiplier);
        baseSpeed *= (1f + evo.speedMultiplier);
        baseFireRate *= (1f + evo.fireRateMultiplier);
        baseBulletSpread *= (1f + evo.bulletSpreadMultiplier);
        baseBulletDamage *= (1f + evo.bulletDamageMultiplier);

        Debug.Log($"[PLAYER EVOLVE] {evo.optionName} → " +
                  $"Health {baseHealth}, Speed {baseSpeed}, FireRate {baseFireRate}, Spread {baseBulletSpread}, Damage {baseBulletDamage}");
        Player.SetStats();
    }
}
