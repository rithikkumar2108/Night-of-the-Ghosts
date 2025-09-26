using UnityEngine;
using System;

public class EvolutionSystem : MonoBehaviour
{
    public static event Action<EvolutionOption> OnPlayerEvolve;
    public static event Action<EvolutionOption> OnEnemiesEvolve;

    public static void EvolvePlayer(EvolutionOption option)
    {
        OnPlayerEvolve?.Invoke(option);
    }

    public static void EvolveEnemies(EvolutionOption option)
    {
        OnEnemiesEvolve?.Invoke(option);
    }
}
